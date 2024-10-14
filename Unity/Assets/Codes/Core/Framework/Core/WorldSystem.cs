using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Text;


namespace Framework
{
    using OneTypeSystems = UnOrderMultiMap<Type,object>;
    
    /// <summary>
    /// ESC核心管理系统，包括初始化，创建Entity，Component
    /// 注册System，以及所有Entity整个生命周期管理
    /// </summary>
    public sealed class WorldSystem : IDisposable
    {
        /// <summary>
        /// 管理每个Entity和其对应的System
        /// 为了接近Unity的生命周期设计
        /// 目前纳入管理的System包括Awake，Load，Destroy，Update等
        /// 后期根据需要可扩展
        /// </summary>
        private class TypeSystem
        {
            /// <summary>
            /// 存储规则： Type ：Entity类型  OneTypeSystems ： SystemType集合
            /// </summary>
            private readonly Dictionary<Type, OneTypeSystems> typeSystemMap = new Dictionary<Type, OneTypeSystems>();

            public OneTypeSystems GetOrCreateOneTypeSystems(Type type)
            {
                OneTypeSystems system = null;
                this.typeSystemMap.TryGetValue(type, out system);
                if (system == null)
                {
                    system = new OneTypeSystems();
                    typeSystemMap.Add(type,system);
                }

                return system;

            }

            public OneTypeSystems GetOneTypeSystem(Type type)
            {
                OneTypeSystems system = null;
                this.typeSystemMap.TryGetValue(type, out system);
                return system;

            }


            /// <summary>
            /// 根据Entity类型和关联的System类型
            /// 根据其不同的life circle处理
            /// </summary>
            /// <param name="type"></param>
            /// <param name="systemType"></param>
            /// <returns></returns>
            public List<object> GetSystems(Type type,Type systemType)
            {
                OneTypeSystems system = null;
                if (!this.typeSystemMap.TryGetValue(type, out system))
                {
                    return null;
                }

                if (!system.TryGetValue(systemType, out List<object> systems))
                {
                    return null;
                }

                return systems;
            }



        }
        
        private static WorldSystem instance;

        public static WorldSystem Instance
        {

            get
            {
                if (instance == null)
                {
                    instance = new WorldSystem();
                }

                return instance;
            }
        }

        #region 各种动态集合

        /// <summary>
        /// 所有目前存在的Entity
        /// </summary>
        private readonly Dictionary<long, Entity> allEntities = new Dictionary<long, Entity>();
        
        /// <summary>
        /// 程序集所有的类型
        /// </summary>
        private readonly Dictionary<string, Type> allTypes = new Dictionary<string, Type>(); 
        /// <summary>
        /// 所有标记了继承BaseAttribute的Attribute的类型,目前是各种System和config object
        /// </summary>
        private readonly UnOrderMultiMap<Type, Type> baseTypes = new UnOrderMultiMap<Type, Type>();

        private readonly Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();


        #endregion

 
        #region  system excute queue
        
        /// <summary>
        /// 这里把所有Update改成分组按分组优先级执行
        /// </summary>
        ///
        public class UpdateGroup
        {
            public byte updatePriority;
            
            public Queue<long> updates = new Queue<long>();
            public Queue<long> updates2 = new Queue<long>();
        }

        private List<UpdateGroup> updateGroups = new List<UpdateGroup>();
        

        private Queue<long> loaders = new Queue<long>();
        private Queue<long> loaders2 = new Queue<long>();
        
        private Queue<long> lateUpdate = new Queue<long>();
        private Queue<long> lateUpdate2 = new Queue<long>();
        
        private Queue<long> fixUpdate = new Queue<long>();
        private Queue<long> fixUpdate2 = new Queue<long>();

        #endregion

        



        private TypeSystem typeSystem = new TypeSystem();
        

        /// <summary>
        /// 将Entity从System调用队列中加入/去除
        /// 这里只需要处理ILoad,IUpdate,ILateupdate
        /// 其余的会在对应触发点主动调用
        /// </summary>
        /// <param name="component"></param>
        /// <param name="beRegister"></param>
        public void RegisterSystem(Entity component, bool beRegister = true)
        {
            if (!beRegister)
            {
                this.RemoveSystem(component.InstanceId);
                
            }
            
            
            this.allEntities.Add(component.InstanceId,component);

            Type type = component.GetType();

            OneTypeSystems oneTypeSystems = typeSystem.GetOneTypeSystem(type);

            if (component is ILoad)
            {
                if (oneTypeSystems.ContainsKey(typeof(ILoadSystem)))
                {
                    this.loaders.Enqueue(component.InstanceId);
                    
                }
            }
            
            if (component is IUpdate)
            {
                if (oneTypeSystems.ContainsKey(typeof(IUpdateSystem)))
                {
                    //this.updates.Enqueue(component.InstanceId);
                    var updateSystem = oneTypeSystems[typeof(IUpdateSystem)][0] as IUpdateSystem;
                    var updateGroup = updateGroups.Find((group =>
                    {
                        return group.updatePriority == updateSystem.SystemPriority;

                    }));

                    if (updateGroup != null)
                    {
                        updateGroup.updates.Enqueue(component.InstanceId);
                    }

                }
            }


            
            if (component is ILateUpdate)
            {
                if (oneTypeSystems.ContainsKey(typeof(ILateUpdateSystem)))
                {
                    this.lateUpdate.Enqueue(component.InstanceId);
                    
                }
            }

            
            if (component is IFixUpdate)
            {
                if (oneTypeSystems.ContainsKey(typeof(IFixUpdateSystem)))
                {
                    this.fixUpdate.Enqueue(component.InstanceId);
                    
                }
            }
            

        }
        

        /// <summary>
        /// 这里不需要移除Entity相应的调用队列
        /// 会在实际循环调用的时候更新队列
        /// </summary>
        /// <param name="instanceId"></param>
        public void RemoveSystem(long instanceId)
        {
            allEntities.Remove(instanceId);

        }

        #region Init types 

        /// <summary>
        /// 获取所有标记了继承BaseAttribute的Attribute的类型
        /// </summary>
        /// <returns></returns>
        private List<Type> GetBaseAttribute()
        {
            List<Type> baseAttributes = new List<Type>();

            foreach (var value in allTypes)
            {
                Type type = value.Value;
                if (type.IsAbstract)
                {
                    continue;
                }

                if (type.IsSubclassOf(typeof(BaseAttribute)))
                {
                    baseAttributes.Add(type);
                    
                }

            }

            return baseAttributes;

        }

        /// <summary>
        /// 初始化各种types，在游戏启动或者需要热重载的时候调用
        /// </summary>
        /// <param name="addTypes"></param>
        public void AddAll(Dictionary<string, Type> addTypes)
        {
            //all types
            this.allTypes.Clear();
            this.updateGroups.Clear();
            foreach (var value in addTypes)
            {
                this.allTypes[value.Key] = value.Value;

            }
            
            //base attribute type
            baseTypes.Clear();
            List<Type> baseAttrubuteTypes = GetBaseAttribute();
            foreach (Type baseAttrubuteType in baseAttrubuteTypes)
            {

                foreach (var kv in allTypes)
                {
                    Type type = kv.Value;

                    if (type.IsAbstract)
                    {
                        continue;
                    }

                    var objects = type.GetCustomAttributes(baseAttrubuteType, true);
                    if (objects.Length == 0)
                    {
                        continue;
                        
                    }
                    
                    this.baseTypes.Add(baseAttrubuteType,type);

                }
                
                
            }
            
            //object system type
            this.typeSystem = new TypeSystem();
            Dictionary<byte, bool> tempUpdateGroups = new Dictionary<byte, bool>();
            foreach (var type in GetTypes(typeof(ObjectSystemAttribute)))
            {
                object obj = Activator.CreateInstance(type);

                if (obj is ISystemBase iSystemBase)
                {
                    //注意这个key type是system对应component的type
                    OneTypeSystems oneTypeSystems = this.typeSystem.GetOrCreateOneTypeSystems(iSystemBase.Type());
                    //这里是system type
                    oneTypeSystems.Add(iSystemBase.SystemType(),obj);
                }
                //初始化update的优先级
                var priorityAtteributes = type.GetCustomAttributes(typeof(SystemUpdatePriorityAttribute),true);
                if(priorityAtteributes.Length == 0)
                    continue;
                SystemUpdatePriorityAttribute  prioritytteribute = priorityAtteributes[0] as SystemUpdatePriorityAttribute;
                var systemBase = obj as ISystemBase;
                systemBase.SystemPriority = prioritytteribute.UpdatePriority;

                tempUpdateGroups[systemBase.SystemPriority] = true;

            }
            
            //创建UpdateGroup列表
            foreach (var tempUpdateGroup in tempUpdateGroups)
            {
                UpdateGroup updateGroup = new UpdateGroup();
                updateGroup.updatePriority = tempUpdateGroup.Key;
                updateGroups.Add(updateGroup);

            }
            
            //排个序
            updateGroups.Sort((group1,group2)=>
            {
                if (group1.updatePriority > group2.updatePriority)
                    return -1;
                if (group1.updatePriority > group2.updatePriority)
                    return 1;

                return 0;

            });
            
            EventSystem.Instance.AddAll(baseTypes[typeof(EventAttribute)]);
            




        }

        /// <summary>
        /// 通过程序集初始化各种类型
        /// </summary>
        /// <param name="assembly"></param>
        public void AddAssembly(Assembly assembly)
        {
            this.assemblies[$"{assembly.GetName().Name}.dll"] = assembly;
            Dictionary<string, Type> allTypes = new Dictionary<string, Type>();

            foreach (var value in  this.assemblies.Values)
            {
                foreach (var type in value.GetTypes())
                {
                    allTypes[type.FullName] = type;

                }
                
            }
            
            this.AddAll(allTypes);

        }

        
        /// <summary>
        /// 获取SystemAttributeType，并不能获取所有类型Type
        /// </summary>
        /// <param name="systemAttributeType"></param>
        /// <returns></returns>
        public List<Type> GetTypes(Type systemAttributeType)
        {
            return this.baseTypes[systemAttributeType];

        }

        public Dictionary<string, Type> GetTypes()
        {
            return allTypes;
        }


        public Type GetType(string typeName)
        {
            return allTypes[typeName];
        }


        #endregion

       

        #region LifeCircel System
        
        /// <summary>
        /// Awake System
        /// </summary>
        /// <param name="compnent"></param>
        public void Awake(Entity component)
        {
            //找到与component相关的system
            List<object> iAwakeSystems =
                this.typeSystem.GetSystems(component.GetType(), typeof(IAwkeSystem));

            if (iAwakeSystems == null)
            {
                return;
                
            }

            for (int i = 0; i < iAwakeSystems.Count; i++)
            {
                IAwkeSystem awakeSystem = iAwakeSystems[i] as IAwkeSystem;
                if(awakeSystem == null)
                    continue;

                try
                {
                    awakeSystem.Run(component);
                }
                catch (Exception e)
                {
                    CustomLogger.Log(LoggerLevel.Error,e.ToString());
                }
            }

            
        }
        
            
        /// <summary>
        /// Awake System
        /// </summary>
        /// <param name="compnent"></param>
        public void Awake<A1>(Entity component,A1 a1)
        {
            //找到与component相关的system
            List<object> iAwakeSystems =
                this.typeSystem.GetSystems(component.GetType(), typeof(IAwkeSystem<A1>));

            if (iAwakeSystems == null)
            {
                return;
                
            }

            for (int i = 0; i < iAwakeSystems.Count; i++)
            {
                IAwkeSystem<A1> awakeSystem = iAwakeSystems[i] as IAwkeSystem<A1>;
                if(awakeSystem == null)
                    continue;

                try
                {
                    awakeSystem.Run(component,a1);
                }
                catch (Exception e)
                {
                    CustomLogger.Log(LoggerLevel.Error,e.ToString());
                }
            }

            
        }
        
        /// <summary>
        /// Awake System
        /// </summary>
        /// <param name="compnent"></param>
        public void Awake<A1,A2>(Entity component,A1 a1,A2 a2)
        {
            //找到与component相关的system
            List<object> iAwakeSystems =
                this.typeSystem.GetSystems(component.GetType(), typeof(IAwkeSystem<A1,A2>));

            if (iAwakeSystems == null)
            {
                return;
                
            }

            for (int i = 0; i < iAwakeSystems.Count; i++)
            {
                IAwkeSystem<A1,A2> awakeSystem = iAwakeSystems[i] as IAwkeSystem<A1,A2>;
                if(awakeSystem == null)
                    continue;

                try
                {
                    awakeSystem.Run(component,a1,a2);
                }
                catch (Exception e)
                {
                    CustomLogger.Log(LoggerLevel.Error,e.ToString());
                }
            }
            
        }
        
        /// <summary>
        /// Awake System
        /// </summary>
        /// <param name="compnent"></param>
        public void Awake<A1,A2,A3>(Entity component,A1 a1,A2 a2,A3 a3)
        {
            //找到与component相关的system
            List<object> iAwakeSystems =
                this.typeSystem.GetSystems(component.GetType(), typeof(IAwkeSystem<A1,A2,A3>));

            if (iAwakeSystems == null)
            {
                return;
                
            }

            for (int i = 0; i < iAwakeSystems.Count; i++)
            {
                IAwkeSystem<A1,A2,A3> awakeSystem = iAwakeSystems[i] as IAwkeSystem<A1,A2,A3>;
                if(awakeSystem == null)
                    continue;

                try
                {
                    awakeSystem.Run(component,a1,a2,a3);
                }
                catch (Exception e)
                {
                    CustomLogger.Log(LoggerLevel.Error,e.ToString());
                }
            }
        }
        
        /// <summary>
        /// Awake System
        /// </summary>
        /// <param name="compnent"></param>
        public void Awake<A1,A2,A3,A4>(Entity component,A1 a1,A2 a2,A3 a3,A4 a4)
        {
            //找到与component相关的system
            List<object> iAwakeSystems =
                this.typeSystem.GetSystems(component.GetType(), typeof(IAwkeSystem<A1,A2,A3,A4>));

            if (iAwakeSystems == null)
            {
                return;
                
            }

            for (int i = 0; i < iAwakeSystems.Count; i++)
            {
                IAwkeSystem<A1,A2,A3,A4> awakeSystem = iAwakeSystems[i] as IAwkeSystem<A1,A2,A3,A4>;
                if(awakeSystem == null)
                    continue;

                try
                {
                    awakeSystem.Run(component,a1,a2,a3,a4);
                }
                catch (Exception e)
                {
                    CustomLogger.Log(LoggerLevel.Error,e.ToString());
                }
            }
            
        }


        /// <summary>
        /// Destroy System
        /// </summary>
        /// <param name="compnent"></param>
        public void Destory(Entity component)
        {
            //找到与component相关的system
            List<object> iDestorySystems =
                this.typeSystem.GetSystems(component.GetType(), typeof(IDestroySystem));

            if (iDestorySystems == null)
            {
                return;
                
            }

            for (int i = 0; i < iDestorySystems.Count; i++)
            {
                IDestroySystem destorySystem = iDestorySystems[i] as IDestroySystem;
                if (destorySystem == null)
                    continue;

                try
                {
                    destorySystem.Run(component);
                }
                catch (Exception e)
                {
                    CustomLogger.Log(LoggerLevel.Error, e.ToString());
                }
            }

        }

        /// <summary>
        /// Deserialize System
        /// </summary>
        /// <param name="component"></param>
        public void Deserialize(Entity component)
        {
            //找到与component相关的system
            List<object> iDeserializeSystems =
                this.typeSystem.GetSystems(component.GetType(), typeof(IDeserializeSystem));

            if (iDeserializeSystems == null)
            {
                return;
                
            }

            for (int i = 0; i < iDeserializeSystems.Count; i++)
            {
                IDeserializeSystem deserializeSystem = iDeserializeSystems[i] as IDeserializeSystem;
                if(deserializeSystem == null)
                    continue;

                try
                {
                    deserializeSystem.Run(component);
                }
                catch (Exception e)
                {
                    CustomLogger.Log(LoggerLevel.Error,e.ToString());
                }
            }


        }

        /// <summary>
        /// AddComponent System
        /// </summary>
        /// <param name="component"></param>
        public void AddComponent(Entity entity, Entity component)
        {
            //找到与component相关的system
            List<object> iAddComponentSystems =
                this.typeSystem.GetSystems(component.GetType(), typeof(IAddComponent));

            if (iAddComponentSystems == null)
            {
                return;
                
            }

            for (int i = 0; i < iAddComponentSystems.Count; i++)
            {
                IAddComponentSystem addComponentSystem = iAddComponentSystems[i] as IAddComponentSystem;
                if(addComponentSystem == null)
                    continue;

                try
                {
                    addComponentSystem.Run(entity,component);
                }
                catch (Exception e)
                {
                    CustomLogger.Log(LoggerLevel.Error,e.ToString());
                }
            }
            
        }

        /// <summary>
        /// GetComponent System
        /// </summary>
        /// <param name="component"></param>
        public void GetComponent(Entity entity, Entity component)
        {
            //找到与component相关的system
            List<object> iGetComponentSystems =
                this.typeSystem.GetSystems(component.GetType(), typeof(IGetComponent));

            if (iGetComponentSystems == null)
            {
                return;
                
            }

            for (int i = 0; i < iGetComponentSystems.Count; i++)
            {
                IGetComponentSystem getComponentSystem = iGetComponentSystems[i] as IGetComponentSystem;
                if(getComponentSystem == null)
                    continue;

                try
                {
                    getComponentSystem.Run(entity,component);
                }
                catch (Exception e)
                {
                    CustomLogger.Log(LoggerLevel.Error,e.ToString());
                }
            }

        }
        
        //以下是队列更新的system
        public void Load()
        {
            while (this.loaders.Count > 0)
            {
                long instanceId = this.loaders.Dequeue();
                Entity component;
                if (!this.allEntities.TryGetValue(instanceId,out component))
                {
                    continue;
                    
                }

                if (component.BeDisposed)
                {
                    continue;
                }
                
                //找到与component相关的system
                List<object> iLoadComponentSystems =
                    this.typeSystem.GetSystems(component.GetType(), typeof(ILoadSystem));

                if (iLoadComponentSystems == null)
                {
                    return;
                
                }
                
                //这里就过滤掉了已经UnRegister的Entity
                loaders2.Enqueue(instanceId);
                
                for (int i = 0; i < iLoadComponentSystems.Count; i++)
                {
                    ILoadSystem loadComponentSystem = iLoadComponentSystems[i] as ILoadSystem;
                    if(loadComponentSystem == null)
                        continue;

                    try
                    {
                        loadComponentSystem.Run(component);
                    }
                    catch (Exception e)
                    {
                        CustomLogger.Log(LoggerLevel.Error,e.ToString());
                    }
                }
              
                
                

            }
            
            ForeachHelper.Swap(ref this.loaders, ref this.loaders2);
        }
        
        public void Update()
        {
            var neddUpdateGroups = updateGroups.FindAll((group) =>
            {
                return group.updates.Count > 0;

            });
            for (int count = 0; count < neddUpdateGroups.Count; count++)
            {
                var updateGroup = updateGroups[count];
                while (updateGroup.updates.Count > 0)
                {
                    long instanceId = updateGroup.updates.Dequeue();
                    Entity component;
                    if (!this.allEntities.TryGetValue(instanceId,out component))
                    {
                        continue;
                    
                    }

                    if (component.BeDisposed)
                    {
                        continue;
                    }
                    
                    //找到与component相关的system
                    List<object> iUpdateComponentSystems =
                        this.typeSystem.GetSystems(component.GetType(), typeof(IUpdateSystem));

                    if (iUpdateComponentSystems == null)
                    {
                        return;
                
                    }
                
                    //这里就过滤掉了已经UnRegister的Entity
                    updateGroup.updates2.Enqueue(instanceId);
                
                    for (int i = 0; i < iUpdateComponentSystems.Count; i++)
                    {
                        IUpdateSystem updateComponentSystem = iUpdateComponentSystems[i] as IUpdateSystem;
                        if(updateComponentSystem == null)
                            continue;
                        
                        if(!component.Enable)
                            continue;

                        try
                        {
                            updateComponentSystem.Run(component);
                        }
                        catch (Exception e)
                        {
                            CustomLogger.Log(LoggerLevel.Error,e.ToString());
                        }
                    }
              
                
                

                }
            
                ForeachHelper.Swap(ref updateGroup.updates, ref updateGroup.updates2);

            }
            
         
        }
        
        
        public void LateUpdate()
        {
            while (this.lateUpdate.Count > 0)
            {
                long instanceId = this.lateUpdate.Dequeue();
                Entity component;
                if (!this.allEntities.TryGetValue(instanceId,out component))
                {
                    continue;
                    
                }

                if (component.BeDisposed)
                {
                    continue;
                }
                
                //找到与component相关的system
                List<object> iLateUpdateComponentSystems =
                    this.typeSystem.GetSystems(component.GetType(), typeof(ILateUpdateSystem));

                if (iLateUpdateComponentSystems == null)
                {
                    return;
                
                }
                
                //这里就过滤掉了已经UnRegister的Entity
                lateUpdate2.Enqueue(instanceId);
                
                for (int i = 0; i < iLateUpdateComponentSystems.Count; i++)
                {
                    ILateUpdateSystem lateUpdateComponentSystem = iLateUpdateComponentSystems[i] as ILateUpdateSystem;
                    if(lateUpdateComponentSystem == null)
                        continue;
                    
                    if(!component.Enable)
                        continue;

                    try
                    {
                        lateUpdateComponentSystem.Run(component);
                    }
                    catch (Exception e)
                    {
                        CustomLogger.Log(LoggerLevel.Error,e.ToString());
                    }
                }
              
                
                

            }
            
            ForeachHelper.Swap(ref this.lateUpdate, ref this.lateUpdate2);
        }
        
        public void FixUpdate()
        {
            while (this.fixUpdate.Count > 0)
            {
                long instanceId = this.fixUpdate.Dequeue();
                Entity component;
                if (!this.allEntities.TryGetValue(instanceId,out component))
                {
                    continue;
                    
                }

                if (component.BeDisposed)
                {
                    continue;
                }
                
                //找到与component相关的system
                List<object> iFixUpdateComponentSystems =
                    this.typeSystem.GetSystems(component.GetType(), typeof(IFixUpdateSystem));

                if (iFixUpdateComponentSystems == null)
                {
                    return;
                
                }
                
                //这里就过滤掉了已经UnRegister的Entity
                fixUpdate2.Enqueue(instanceId);
                
                for (int i = 0; i < iFixUpdateComponentSystems.Count; i++)
                {
                    IFixUpdateSystem fixUpdateComponentSystem = iFixUpdateComponentSystems[i] as IFixUpdateSystem;
                    if(fixUpdateComponentSystem == null)
                        continue;
                    
                    if(!component.Enable)
                        continue;

                    try
                    {
                        fixUpdateComponentSystem.Run(component);
                    }
                    catch (Exception e)
                    {
                        CustomLogger.Log(LoggerLevel.Error,e.ToString());
                    }
                }
              
                
                

            }
            
            ForeachHelper.Swap(ref this.fixUpdate, ref this.fixUpdate2);
        }


        #endregion



        #region entity operation

        /// <summary>
        /// 一般内部调用
        /// </summary>
        /// <param name="instanceId"></param>
        public void RemoveEntity(long instanceId)
        {
            this.allEntities.Remove(instanceId);
        }

        public Entity Get(long instanceId)
        {
            this.allEntities.TryGetValue(instanceId, out Entity component);
            return component;
        }

        public bool IsRegister(long instanceId)
        {
            return this.allEntities.ContainsKey(instanceId);
        }

        #endregion


        public  void Dispose()
        {
            instance = null;

        }
        
        public override string ToString()
        {

            using (var sb = ZString.CreateStringBuilder())
            {
                HashSet<Type> noParent = new HashSet<Type>();
                Dictionary<Type, int> typeCount = new Dictionary<Type, int>();

                HashSet<Type> noDomain = new HashSet<Type>();

                foreach (var kv in this.allEntities)
                {
                    Type type = kv.Value.GetType();
                    if (kv.Value.Parent == null)
                    {
                        noParent.Add(type);
                    }

                    if (kv.Value.Domain == null)
                    {
                        noDomain.Add(type);
                    }

                    if (typeCount.ContainsKey(type))
                    {
                        typeCount[type]++;
                    }
                    else
                    {
                        typeCount[type] = 1;
                    }
                }

                sb.AppendLine("not set parent type: ");
                foreach (Type type in noParent)
                {
                    sb.AppendLine($"\t{type.Name}");
                }

                sb.AppendLine("not set domain type: ");
                foreach (Type type in noDomain)
                {
                    sb.AppendLine($"\t{type.Name}");
                }

                IOrderedEnumerable<KeyValuePair<Type, int>> orderByDescending = typeCount.OrderByDescending(s => s.Value);

                sb.AppendLine("Entity Count: ");
                foreach (var kv in orderByDescending)
                {
                    if (kv.Value == 1)
                    {
                        continue;
                    }

                    sb.AppendLine($"\t{kv.Key.Name}: {kv.Value}");
                }

                return sb.ToString();
                
            }

           
        }

    }
}