using System;

namespace Framework
{
    // GetComponentSystem有巨大作用，比如每次保存Unit的数据不需要所有组件都保存，
    // 只需要保存Unit变化过的组件,是否变化可以通过判断该组件是否GetComponent，
    // Get了就记录该组件这样可以只保存Unit变化过的组件
    public interface IGetComponent
    {
        
    }
    

    public interface IGetComponentSystem : ISystemBase
    {

        void Run(object o,Entity component);

    }
        
    //---------------后有需要再来扩展泛型个数--------------------//
    

    [ObjectSystem]

    public abstract class GetComponentSystem<T> : IGetComponentSystem where T : IGetComponent
    {
        public void Run(object o,Entity component)
        {
            this.GetComponent((T)o,component);
        }

        public abstract void GetComponent(T self,Entity component);
       

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
            return typeof(IGetComponentSystem);

        }
        
        public Byte SystemPriority
        {
            get;
            set;
        }

    }
}