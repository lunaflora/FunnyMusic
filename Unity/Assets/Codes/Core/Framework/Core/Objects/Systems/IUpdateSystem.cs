using System;

namespace Framework
{
    public interface IUpdate
    {
        
    }
    

    public interface IUpdateSystem : ISystemBase
    {

        void Run(object o);

    }
        
    //---------------后有需要再来扩展泛型个数--------------------//

    [ObjectSystem]
    [SystemUpdatePriority(UpdatePriority = 0)]
    public abstract class UpdateSystem<T> : IUpdateSystem where T : IUpdate
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
            return typeof(IUpdateSystem);

        }
        
        public Byte SystemPriority
        {
            get;
            set;
        }

    }
}