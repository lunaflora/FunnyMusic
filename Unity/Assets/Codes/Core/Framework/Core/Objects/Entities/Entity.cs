using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using CatJson;
using Unity.VisualScripting;

namespace Framework
{
    public partial class Entity : DynamicObject
    {

        /// <summary>
        /// 自动Generate的id
        /// </summary>
        [IgnoreDataMember]
        [JsonIgnore]
        public long InstanceId
        {
            get;
            protected set;
        }

        /// <summary>
        /// 由外部指定的逻辑ID
        /// </summary>
        public long Id
        {
            get;
            set;
        }
        
        protected Entity(){
        }


        [IgnoreDataMember] 
        [JsonIgnore]
        private EntityStatus entityStatus = EntityStatus.None;

        #region EntityStatus Function

        [IgnoreDataMember]
        [JsonIgnore]
        private bool BePooled
        {
            get => (this.entityStatus & EntityStatus.BePooled) == EntityStatus.BePooled;
            
            set
            {
                if (value)
                {
                    this.entityStatus |= EntityStatus.BePooled;
                }
                else
                {
                    this.entityStatus &= ~EntityStatus.BePooled;
                }
               
            }
        }
        
        /// <summary>
        /// 在设置Domain和Dispose时标记
        /// </summary>
        [IgnoreDataMember]
        [JsonIgnore]
        protected bool BeRegister
        {
            get => (this.entityStatus & EntityStatus.BeRegister) == EntityStatus.BeRegister;
            
            set
            {
                if (value)
                {
                    this.entityStatus |= EntityStatus.BeRegister;
                }
                else
                {
                    this.entityStatus &= ~EntityStatus.BeRegister;
                }
                
                WorldSystem.Instance.RegisterSystem(this,value);
               
            }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        private bool BeComponent
        {
            get => (this.entityStatus & EntityStatus.BeComponent) == EntityStatus.BeComponent;

            set
            {
                if (value)
                {
                    this.entityStatus |= EntityStatus.BeComponent;
                }
                else
                {
                    this.entityStatus &= ~EntityStatus.BeComponent;
                }
            }

        }
        
        [IgnoreDataMember]
        [JsonIgnore]
        protected bool BeCreated
        {
            get => (this.entityStatus & EntityStatus.BeCreated) == EntityStatus.BeCreated;

            set
            {
                if (value)
                {
                    this.entityStatus |= EntityStatus.BeCreated;
                }
                else
                {
                    this.entityStatus &= ~EntityStatus.BeCreated;
                }
            }

        }
        [IgnoreDataMember]
        [JsonIgnore]
        protected bool BeNew
        {
            get => (this.entityStatus & EntityStatus.BeNew) == EntityStatus.BeNew;

            set
            {
                if (value)
                {
                    this.entityStatus |= EntityStatus.BeNew;
                }
                else
                {
                    this.entityStatus &= ~EntityStatus.BeNew;
                }
            }

        }

        [IgnoreDataMember]
        [JsonIgnore]
        protected bool BeJobs
        {
            get
            {
                if((this.entityStatus & EntityStatus.BeJobs) != EntityStatus.BeJobs)
                {
                    var bJobs = this is ICustomJob;
                    if (bJobs)
                    {
                        this.entityStatus |= EntityStatus.BeJobs;
                    }
                    else
                    {
                        this.entityStatus &= ~EntityStatus.BeJobs;
                    }
                }
             
                
                return (this.entityStatus & EntityStatus.BeJobs) == EntityStatus.BeJobs;
            }
        }

        #endregion

        
        [IgnoreDataMember]
        [JsonIgnore]
        public bool BeDisposed => this.InstanceId == 0;

        [IgnoreDataMember]
        [JsonIgnore]
        protected Entity parent;
        
        [IgnoreDataMember]
        [JsonIgnore]
        protected Entity domain;

        /// <summary>
        /// Entity的根节点，方便随时获取World
        /// 这个一定要有，set操作只在Parent，ComponentParent中调用
        /// </summary>
        public Entity Domain
        
        
        {
            get => this.domain;
            private set
            {
                if (value == null)
                {
                    throw new Exception($"Entity can not set domain null : {this.GetType().Name}");
                }

                if (value == this.domain)
                {
                   return;
                }

                Entity preDomain = this.domain;
                this.domain = value;

                //如果没有domain，代表是新创建的
                if (preDomain == null)
                {
                    this.InstanceId = IdGenerater.Instance.GenerateInstanceId();
                    this.BeRegister = true;

                    if (this.componentsDB != null)
                    {
                        this.componentsDB.Foreach(component =>
                        {
                            component.BeComponent = true;
                            this.components.Add(component.GetType(),component);
                            component.parent = this;

                        });
                        
                    }
                    
                    if (this.childrenDB != null)
                    {
                        this.childrenDB.Foreach(child =>
                        {
                            child.BeComponent = false;
                            this.children.Add(child.Id,child);
                            child.parent = this;

                        });
                    }

                }
                
                //这里需要递归设置子Entity的Domain

                if (this.children != null)
                {
                    this.children.Foreach((_,child) =>
                    {
                        child.Domain = this.domain;
                    });
                }
                
                if (this.components != null)
                {
                    this.components.Foreach((_,component) =>
                    {
                        component.Domain = this.domain;
                    });
                }

                //这里给反序列化留个接口，如果不是Create出来的，就是从数据库或者存档
                //中获取的，需要反序列化，具体等做持久化功能的时候再定
                if (!this.BeCreated)
                {
                    this.BeCreated = true;
                    WorldSystem.Instance.Deserialize(this);
                    
                }


            }
            
          
            
        }

        /// <summary>
        /// Parent一定不能为null
        /// 这个设置在AddChild系列中被调用
        /// </summary>
        [IgnoreDataMember]
        [JsonIgnore]
        public Entity Parent
        {

            get => this.parent;

            private set
            {
                if (value == null)
                {
                    throw new Exception($"Entity can not set parent null : {this.GetType().Name}");
                }

                if (value == this)
                {
                    throw new Exception($"Entity can not set parent itself : {this.GetType().Name}");
                }
                
                //parent必须要有domain,如果修改了parent,那么自身的domain也要和新的parent一致
                if (value.Domain == null)
                {
                    throw new Exception($"Entity can not set parent because parent domain is null : " +
                                        $"{this.GetType().Name}  {value.GetType().Name}");
                }

                if (this.parent != null)
                {
                    if (this.parent == value)
                    {
                        CustomLogger.Log(LoggerLevel.Warning,$"Entity set a same parent :" +
                                                             $" {this.GetType().Name}  {value.GetType().Name} ");
                        return;
                    }
                    
                    //如果有以前的parent，需要从parent移除自己
                    this.parent.RemoveFromChildren(this);
                }

                this.parent = value;
                //如果设置了parent，自己就是一个Entity，Component参看ComponentParent
                this.BeComponent = false;
                this.parent.AddToChildren(this);
                this.Domain = parent.domain;

            }
        }


        /// <summary>
        /// 这里主要和parent区分，通过这种方式挂载的Entity是Component
        /// this.BeComponent = true;
        /// 这个设置在AddComponent中被调用
        /// </summary>
        [JsonIgnore]
        private Entity ComponentParent
        {
            set
            {
                if (value == null)
                {
                    throw new Exception($"Component can not set parent null : {this.GetType().Name}");
                }

                if (value == this)
                {
                    throw new Exception($"Component can not set parent itself : {this.GetType().Name}");
                }
                
                //parent必须要有domain,如果修改了parent,那么自身的domain也要和新的parent一致
                if (value.Domain == null)
                {
                    throw new Exception($"Component can not set parent because parent domain is null : " +
                                        $"{this.GetType().Name}  {value.GetType().Name}");
                }

                if (this.parent != null)
                {
                    if (this.parent == value)
                    {
                        CustomLogger.Log(LoggerLevel.Warning,$"Component set a same parent :" +
                                                             $" {this.GetType().Name}  {value.GetType().Name} ");
                        return;
                    }
                    
                    //如果有以前的parent，需要从parent移除自己
                    this.parent.RemoveFromComponents(this);
                }

                this.parent = value;
                //如果设置了ComponentParent，自己就是一个Component，Entity参看Parent
                this.BeComponent = true;
                this.parent.AddToComponents(this);
                this.Domain = parent.domain;

            }
        }

        public T GetParent<T>() where T : Entity
        {
            return this.parent as T;
        }

        /// <summary>
        /// 先放这里，后面做网络同步需要
        /// </summary>
        [IgnoreDataMember] 
        [JsonIgnore]
        private HashSet<Entity> childrenDB;
        
        /// <summary>
        /// children从逻辑上是自身的子Entity，和components区分
        /// </summary>
        [IgnoreDataMember] 
        [JsonIgnore]
        private Dictionary<long,Entity> children;
        
        [IgnoreDataMember] 
        [JsonIgnore]

        public Dictionary<long, Entity> Children
        {
            get
            {
                if (children == null)
                {
                    children = MonoPool.Instance.Fetch<Dictionary<long, Entity>>();
                }

                return children;
            }
        }

        /// <summary>
        /// 先放这里，后面做网络同步需要
        /// </summary>
        [IgnoreDataMember]
        [JsonIgnore]
        private HashSet<Entity> componentsDB;

        /// <summary>
        /// components是挂载在Entity上的组件 和 children区分
        /// </summary>
        [IgnoreDataMember]
        [JsonIgnore]
        private Dictionary<Type, Entity> components;

        [IgnoreDataMember]
        [JsonIgnore]
        public Dictionary<Type, Entity> Components
        {
            get
            {
                if (components == null)
                {
                    components = MonoPool.Instance.Fetch<Dictionary<Type, Entity>>();
                }

                return components;
            }
        }

        [IgnoreDataMember] 
        [JsonIgnore] 
        private bool enable = true;

        public bool Enable
        {
            get => enable;
            set => enable = value;
        }

        private void AddToComponents(Entity component)
        {
            if (!this.Components.TryAdd(component.GetType(), component))
            {
                throw new Exception($"Add a same type component to entity : " +
                                    $"entity : {this.Id}  children {component.GetType()}");
            }
        }

        /// <summary>
        /// 移除挂在的组件,这里并不会Dispose这个组件
        /// </summary>
        private void RemoveFromComponents(Entity component)
        {
            if (this.components == null)
            {
                return;
            }

            this.components.Remove(component.GetType());

            if (this.components.Count == 0)
            {
                MonoPool.Instance.Recycle(this.components);
                this.components = null;
            }
            

        }

        private void AddToChildren(Entity entity)
        {
            if (!this.Children.TryAdd(entity.Id, entity))
            {
                throw new Exception($"Add a same id children to entity : " +
                                    $"parent : {this.Id}  children {entity.Id}");
            }
            
        }

        private void RemoveFromChildren(Entity entity)
        {
            if (this.children == null)
            {
                return;
            }

            this.children.Remove(entity.Id);

            if (this.children.Count == 0)
            {
                MonoPool.Instance.Recycle(this.children);
                this.children = null;
            }

        }
        


        public override void Dispose()
        {
            if (this.BeDisposed)
            {
                return;
            }

            this.InstanceId = 0;
            this.BeRegister = false;
            
            //递归清理Components
            if (this.components != null)
            {
                this.components.Foreach((_, entity) =>
                {
                    entity.Dispose();
                });
                
                this.components.Clear();
                MonoPool.Instance.Recycle(this.components);
                this.components = null;
            }
            
            //递归清理Chidren
            if (this.children != null)
            {
                this.children.Foreach((_, child) =>
                {
                    child.Dispose();
                });
                
                this.children.Clear();
                MonoPool.Instance.Recycle(this.children);
                this.children = null;
            }
            
            //触发DestroySystem
            if (this is IDestroy)
            {
                WorldSystem.Instance.Destory(this);
            }
            
            this.domain = null;
            
            //要从父节点中删除这个Entity，以便gc回收
            if (this.parent != null && !this.parent.BeDisposed)
            {
                if (this.BeComponent)
                {
                    this.parent.RemoveComponent(this);
                }
                else
                {
                    this.parent.RemoveFromChildren(this);
                }
                
            }

            this.parent = null;
            base.Dispose();
            
            //如果是缓存了的，需要回收缓存池，以免被gc释放
            //缓存一般用于大量重复性单位
            if (this.BePooled)
            {
                EntityPool.Instance.Recycle(this);
            }

            entityStatus = EntityStatus.None;

        }


        #region 各种添加Entity，删除Entity的操作

        private static Entity Create(Type type, bool bePooled)
        {
            Entity component;
            if (bePooled)
            {
                component = EntityPool.Instance.Fetch(type);
            }
            else
            {
                component = Activator.CreateInstance(type) as Entity;
            }

            component.BePooled = bePooled;
            component.BeCreated = true;
            component.BeNew = true;
            component.Id = 0;

            return component;
        }

        public Entity AddComponent(Entity component)
        {
            Type type = component.GetType();
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"Entity already has this component {type.FullName}!");
            }

            //这个触发AddSystem
            if (this is IAddComponent)
            {
                WorldSystem.Instance.AddComponent(this,component);
            }

            return component;
        }
        
        public Entity AddComponent(Type type, bool bePool = false)
        {
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"Entity already has this component {type.FullName}!");
            }

            Entity component = Create(type, bePool);
            //component的ID就是对应Entity的ID
            component.Id = this.Id;
            //这里也设置了parent
            component.ComponentParent = this;
            //触发AwakeSytem,如果有的话
            WorldSystem.Instance.Awake(component);

            //这个触发AddSystem
            if (this is IAddComponent)
            {
                WorldSystem.Instance.AddComponent(this,component);
            }

            return component;
        }

        /// <summary>
        /// 几个泛型,参数的扩展
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bePool"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public K AddComponent<K>(bool bePool = false)where K : Entity,IAwake,new()
        {
            Type type = typeof(K);
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"Entity already has this component {type.FullName}!");
            }

            Entity component = Create(type, bePool);
            //component的ID就是对应Entity的ID
            component.Id = this.Id;
            //这里也设置了parent
            component.ComponentParent = this;
            //触发AwakeSytem,如果有的话
            WorldSystem.Instance.Awake(component);

            //这个触发AddSystem
            if (this is IAddComponent)
            {
                WorldSystem.Instance.AddComponent(this,component);
            }

            return component as K;
        }
        /// <summary>
        /// 几个泛型,参数的扩展
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bePool"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public K AddComponent<K,A1>(A1 a1,bool bePool = false)where K : Entity,IAwake<A1>,new()
        {
            Type type = typeof(K);
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"Entity already has this component {type.FullName}!");
            }

            Entity component = Create(type, bePool);
            //component的ID就是对应Entity的ID
            component.Id = this.Id;
            //这里也设置了parent
            component.ComponentParent = this;
            //触发AwakeSytem,如果有的话
            WorldSystem.Instance.Awake(component,a1);

            //这个触发AddSystem
            if (this is IAddComponent)
            {
                WorldSystem.Instance.AddComponent(this,component);
            }

            return component as K;
        }
        
        public K AddComponent<K,A1,A2>(A1 a1,A2 a2,bool bePool = false)where K : Entity,IAwake<A1,A2>,new()
        {
            Type type = typeof(K);
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"Entity already has this component {type.FullName}!");
            }

            Entity component = Create(type, bePool);
            //component的ID就是对应Entity的ID
            component.Id = this.Id;
            //这里也设置了parent
            component.ComponentParent = this;
            //触发AwakeSytem,如果有的话
            WorldSystem.Instance.Awake(component,a1,a2);

            //这个触发AddSystem，注意这里触发的this的AddComponent
            if (this is IAddComponent)
            {
                WorldSystem.Instance.AddComponent(this,component);
            }

            return component as K;
        }
        
        public K AddComponent<K,A1,A2,A3>(A1 a1,A2 a2, A3 a3,bool bePool = false)where K : Entity,IAwake<A1,A2,A3>,new()
        {
            Type type = typeof(K);
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"Entity already has this component {type.FullName}!");
            }

            Entity component = Create(type, bePool);
            //component的ID就是对应Entity的ID
            component.Id = this.Id;
            //这里也设置了parent
            component.ComponentParent = this;
            //触发AwakeSytem,如果有的话
            WorldSystem.Instance.Awake(component,a1,a2,a3);

            //这个触发AddComponentSystem
            if (this is IAddComponent)
            {
                WorldSystem.Instance.AddComponent(this,component);
            }

            return component as K;
        }

        public Entity AddChild(Entity entity)
        {

            entity.Parent = this;
            return entity;
        }

        public T AddChild<T>(bool bePool = false) where T : Entity, IAwake
        {
            Type type = typeof(T);
            T child = (T)Create(type, bePool);
            child.Id = IdGenerater.Instance.GenerateId();
            child.Parent = this;
            
            WorldSystem.Instance.Awake(child);

            return child;
        }
        
        
        public T AddChild<T>() where T : Entity
        {
            Type type = typeof(T);
            T child = (T)Create(type, false);
            child.Id = IdGenerater.Instance.GenerateId();
            child.Parent = this;
            
            WorldSystem.Instance.Awake(child);

            return child;
        }
        public T AddChild<T,A1>(A1 a1,bool bePool = false) where T : Entity, IAwake<A1>
        {
            Type type = typeof(T);
            T child = (T)Create(type, bePool);
            child.Id = IdGenerater.Instance.GenerateId();
            child.Parent = this;
            
            WorldSystem.Instance.Awake(child,a1);

            return child;
        }
        public T AddChild<T,A1,A2>(A1 a1, A2 a2, bool bePool = false) where T : Entity, IAwake<A1,A2>
        {
            Type type = typeof(T);
            T child = (T)Create(type, bePool);
            child.Id = IdGenerater.Instance.GenerateId();
            child.Parent = this;
            
            WorldSystem.Instance.Awake(child,a1,a2);

            return child;
        }
        
        public T AddChild<T,A1,A2,A3>(A1 a1,A2 a2,A3 a3,bool bePool = false) where T : Entity, IAwake<A1,A2,A3>
        {
            Type type = typeof(T);
            T child = (T)Create(type, bePool);
            child.Id = IdGenerater.Instance.GenerateId();
            child.Parent = this;
            
            WorldSystem.Instance.Awake(child,a1,a2,a3);

            return child;
        }
        
        public T AddChild<T,A1,A2,A3,A4>(A1 a1,A2 a2,A3 a3,A4 a4,bool bePool = false) where T : Entity, IAwake<A1,A2,A3,A4>
        {
            Type type = typeof(T);
            T child = (T)Create(type, bePool);
            child.Id = IdGenerater.Instance.GenerateId();
            child.Parent = this;
            
            WorldSystem.Instance.Awake(child,a1,a2,a3,a4);

            return child;
        }
        
        
        public T AddChildWithId<T>(long id,bool bePool = false) where T : Entity, IAwake
        {
            Type type = typeof(T);
            T child = (T)Create(type, bePool);
            child.Id = id;
            child.Parent = this;
            
            WorldSystem.Instance.Awake(child);

            return child;
        }
        
        public T AddChildWithId<T,A1>(long id,A1 a1,bool bePool = false) where T : Entity, IAwake<A1>
        {
            Type type = typeof(T);
            T child = (T)Create(type, bePool);
            child.Id = id;
            child.Parent = this;
            
            WorldSystem.Instance.Awake(child,a1);

            return child;
        }
        public T AddChildWithId<T,A1,A2>(long id,A1 a1, A2 a2, bool bePool = false) where T : Entity, IAwake<A1,A2>
        {
            Type type = typeof(T);
            T child = (T)Create(type, bePool);
            child.Id = id;
            child.Parent = this;
            
            WorldSystem.Instance.Awake(child,a1,a2);

            return child;
        }
        
        public T AddChildWithId<T,A1,A2,A3>(long id,A1 a1,A2 a2,A3 a3,bool bePool = false) where T : Entity, IAwake<A1,A2,A3>
        {
            Type type = typeof(T);
            T child = (T)Create(type, bePool);
            child.Id = id;
            child.Parent = this;
            
            WorldSystem.Instance.Awake(child,a1,a2,a3);

            return child;
        }
        
        public T AddChildWithId<T,A1,A2,A3,A4>(long id,A1 a1,A2 a2,A3 a3,A4 a4,bool bePool = false) where T : Entity, IAwake<A1,A2,A3,A4>
        {
            Type type = typeof(T);
            T child = (T)Create(type, bePool);
            child.Id = id;
            child.Parent = this;
            
            WorldSystem.Instance.Awake(child,a1,a2,a3,a4);

            return child;
        }

        public K GetChild<K>(long id) where K : Entity
        {
            if (this.children == null)
            {
                return null;
            }

            if (!this.children.TryGetValue(id, out Entity child))
            {
                throw new Exception($"{this.GetType()} dos not have child id {id}!");
            }

            return child as K;

        }
        
        public K GetChildAllowNone<K>(long id) where K : Entity
        {
            if (this.children == null)
            {
                return null;
            }

            if (!this.children.TryGetValue(id, out Entity child))
            {
                return null;
            }

            return child as K;

        }

        public Entity GetComponent(Type type)
        {
            if (this.components == null)
            {
                return null;
            }

            if (!this.components.TryGetValue(type, out Entity component))
            {
                return null;
            }

            if (this is IGetComponent)
            {
                WorldSystem.Instance.GetComponent(this,component);
                
            }


            return component;

        }
        
        public K GetComponent<K>() where K : Entity
        {
            if (this.components == null)
            {
                return null;
            }

            if (!this.components.TryGetValue(typeof(K), out Entity component))
            {
                return null;
            }

            if (this is IGetComponent)
            {
                WorldSystem.Instance.GetComponent(this,component);
                
            }


            return component as K;

        }
        
        public void RemoveComponent(Entity component)
        {
            if (this.BeDisposed)
            {
                return;
            }

            if (this.components == null)
            {
                return;
            }

            Type type = component.GetType();
            Entity c = GetComponent(type);
            
            if(c == null)
                return;

            if (c.InstanceId != component.InstanceId)
            {
                return;
            }
            
            
            RemoveFromComponents(c);
            c.Dispose();
            
        }

        public void RemoveComponent<K>() where K : Entity
        {
            if (this.BeDisposed)
            {
                return;
            }

            if (this.components == null)
            {
                return;
            }

            Type type = typeof(K);
            Entity c = GetComponent(type);
            
            if(c == null)
                return;
            
            
            RemoveFromComponents(c);
            c.Dispose();
            
        }
        
        public void RemoveComponent(Type type)
        {
            if (this.BeDisposed)
            {
                return;
            }

            if (this.components == null)
            {
                return;
            }
            
            Entity c = GetComponent(type);
            
            if(c == null)
                return;
            
            
            RemoveFromComponents(c);
            c.Dispose();
            
        }
        
        
        


        #endregion
    }

    [Flags]
    public enum EntityStatus : byte
    {
        None = 0,
        //已经被缓存
        BePooled = 1,
        //新创建的,目前都是
        BeNew = 1 << 1,
        //已经注册到WorldSystem里面的
        //主要用于pool
        BeRegister = 1 << 2,
        //是Component还是Child Entity
        BeComponent = 1 << 3,
        //是否是Create出来的，不是则需要反序列化的
        //主要用于来自于从存档和数据库的数据
        //这里mark一下，一个Entity存档可以序列化成Component列表+child列表的形式，然后反序列化组装
        //{T : ComponentData}
        //通过字段属性和类接口区分是否参与正反序列化
        BeCreated = 1 << 4,
        //是否需要多线程处理，一般用于rendering object
        BeJobs = 1 << 5
        
        
    }
}