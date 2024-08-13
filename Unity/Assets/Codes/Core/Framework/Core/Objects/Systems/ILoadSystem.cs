using System;

namespace Framework
{
    public interface ILoad
    {
        
    }
    
    /// <summary>
    /// 通常用于整个游戏周期只执行一次的Load
    /// </summary>
    public interface ILoadSystem : ISystemBase
    {

        void Run(object o);

    }

    [ObjectSystem]

    public abstract class LoadSystem<T> : ILoadSystem where T : ILoad
    {
        public void Run(object o)
        {
            this.Load((T)o);
        }

        public abstract void Load(T self);
       

        /// <summary>
        /// 这里是获取对应Entity的类型
        /// </summary>
        /// <returns></returns>
        public Type Type()
        {
            return typeof(T);
        }

        /// <summary>
        /// 这里才是获取System的type
        /// </summary>
        /// <returns></returns>
        public Type SystemType()
        {
            return typeof(ILoadSystem);

        }
        
        public Byte SystemPriority
        {
            get;
            set;
        }

    }
}