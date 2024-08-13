using System.Collections.Generic;
using System.Reflection;

namespace Framework
{
    /// <summary>
    /// init和update整个WorldSystem
    /// 游戏世界生命周期
    /// </summary>
    public static class GameWorld
    {
        
        public static TimeInfo TimeInfo => TimeInfo.Instance;
        public static IdGenerater IdGenerater => IdGenerater.Instance;
        
        public static WorldSystem WorldSystem => WorldSystem.Instance;

        public static string WorldName;

        private static World world;

        public static World World
        {
            get
            {
                if (world == null)
                {
                    world = CreateWorld(IdGenerater.Instance.GenerateInstanceId(), WorldName);
                }

                return world;
            }
            
        }

        /// <summary>
        /// 以下更新函数在mono中调用
        /// 目的是和mono解耦
        /// </summary>

        public static void Start(List<Assembly> assemblies,string worldName = "GameWorld")
        {
            Init(assemblies);
            WorldName = worldName;
        }
        
        public static void Update(){
            
            TimeInfo.Update();
            WorldSystem.Update();
        }

        public static void LateUpdate()
        {
            WorldSystem.LateUpdate();
        }

        private static void Init(List<Assembly> assemblies)
        {
            //初始化WorldSystem
            WorldSystem.Instance.AddAssembly(typeof(GameWorld).Assembly);
            foreach (var assembly in assemblies)
            {
                WorldSystem.Instance.AddAssembly(assembly);
            }
            //初始化EventSystem
            
            EventSystem.Instance.TriggerSync(new EventType.GameInit());

        }
        
        private static World CreateWorld(long id,string name, Entity parent = null)
        {
            World world = new World(id, name, parent);
            return world;
        }

    }
}