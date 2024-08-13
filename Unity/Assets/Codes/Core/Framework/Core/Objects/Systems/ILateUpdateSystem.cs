using System;

namespace Framework
{
    public interface ILateUpdate
    {
        
    }
    

    public interface ILateUpdateSystem : ISystemBase
    {

        void Run(object o);

    }
        
    //---------------后有需要再来扩展泛型个数--------------------//

    [ObjectSystem]

    public abstract class LateUpdateSystem<T> : ILateUpdateSystem where T : ILateUpdate
    {
        public void Run(object o)
        {
            this.LateUpdate((T)o);
        }

        public abstract void LateUpdate(T self);
       

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
            return typeof(ILateUpdateSystem);

        }
        
        public Byte SystemPriority
        {
            get;
            set;
        }

    }
}