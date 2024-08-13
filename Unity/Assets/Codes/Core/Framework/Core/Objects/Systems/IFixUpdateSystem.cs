using System;

namespace Framework
{
    public interface IFixUpdate
    {
        
    }
    
    public interface IFixUpdateSystem : ISystemBase
    {
        void Run(object o);
        
    }
    
    
    [ObjectSystem]

    public abstract class FixUpdateSystem<T> : IFixUpdateSystem where T : IFixUpdate
    {
        public void Run(object o)
        {
            this.Update((T)o);
        }

        public abstract void Update(T self);
       

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
            return typeof(IFixUpdateSystem);

        }

        public Byte SystemPriority
        {
            get;
            set;
        }

    }
}