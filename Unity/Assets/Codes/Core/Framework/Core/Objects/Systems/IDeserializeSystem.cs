using System;

namespace Framework
{
    public interface IDeserialize
    {
        
    }
    

    public interface IDeserializeSystem : ISystemBase
    {

        void Run(object o);

    }
        
    //---------------后有需要再来扩展泛型个数--------------------//
    
    /// <summary>
    /// 反序列化system，实现各自的反序列化方法
    /// </summary>
    /// <typeparam name="T"></typeparam>

    [ObjectSystem]

    public abstract class DeserializeSystem<T> : IDeserializeSystem where T : IDeserialize
    {
        public void Run(object o)
        {
            this.Deserialize((T)o);
        }

        public abstract void  Deserialize(T self);
       

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
            return typeof(IDeserializeSystem);

        }
        
        public Byte SystemPriority
        {
            get;
            set;
        }

    }
}