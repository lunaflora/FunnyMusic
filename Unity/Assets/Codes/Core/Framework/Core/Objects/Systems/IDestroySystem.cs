using System;

namespace Framework
{
    public interface IDestroy
    {
        
    }
    

    public interface IDestroySystem : ISystemBase
    {

        void Run(object o);

    }
        
    //---------------后有需要再来扩展泛型个数--------------------//

    [ObjectSystem]

    public abstract class DestroySystem<T> : IDestroySystem where T : IDestroy
    {
        public void Run(object o)
        {
            this.Destroy((T)o);
        }

        public abstract void Destroy(T self);
       

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
            return typeof(IDestroySystem);

        }
        
        public Byte SystemPriority
        {
            get;
            set;
        }

    }
}