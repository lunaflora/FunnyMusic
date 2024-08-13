using System;

namespace Framework
{
    public interface IAddComponent
    {
        
    }
    

    public interface IAddComponentSystem : ISystemBase
    {

        void Run(object o,Entity component);

    }
        
    //---------------后有需要再来扩展泛型个数--------------------//
    
    /// <summary>
    /// 有时候需要在一个Entity新增Component时进行一些额外处理
    /// 比如触发数据保存
    /// </summary>
    /// <typeparam name="T"></typeparam>

    [ObjectSystem]

    public abstract class AddComponentSystem<T> : IAddComponentSystem where T : IAddComponent
    {
        public void Run(object o,Entity component)
        {
            this.AddComponent((T)o,component);
        }

        public abstract void AddComponent(T self,Entity component);
       

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
            return typeof(IAddComponentSystem);

        }
        
        public Byte SystemPriority
        {
            get;
            set;
        }

    }
}