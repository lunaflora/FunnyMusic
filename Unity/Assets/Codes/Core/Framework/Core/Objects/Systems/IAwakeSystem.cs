using System;

namespace Framework
{
    public interface IAwake
    {
        
    }
    
    public interface IAwake<A>
    {
        
    }
    
    public interface IAwake<A,B>
    {
        
    }
    
    public interface IAwake<A,B,C>
    {
        
    }
    
    public interface IAwake<A,B,C,D>
    {
        
    }
    
    //----------------------不够再说------------------//
    

    public interface IAwkeSystem : ISystemBase
    {

        void Run(object o);

    }
    public interface IAwkeSystem<A> : ISystemBase
    {

        void Run(object o, A a);

    }
    public interface IAwkeSystem<A,B> : ISystemBase
    {

        void Run(object o, A a, B b);

    }
    public interface IAwkeSystem<A,B,C> : ISystemBase
    {

        void Run(object o, A a, B b, C c);

    }
    public interface IAwkeSystem<A,B,C,D> : ISystemBase
    {

        void Run(object o,A a, B b, C c, D d);

    }

    [ObjectSystem]

    public abstract class AwakeSystem<T> : IAwkeSystem where T : IAwake
    {
        public void Run(object o)
        {
            this.Awake((T)o);
        }

        public abstract void Awake(T self);
       

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
            return typeof(IAwkeSystem);

        }
        
        public Byte SystemPriority
        {
            get;
            set;
        }

    }
    /// <summary>
    /// T是Component/Entity,A是Parameter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="A"></typeparam>
    [ObjectSystem]

    public abstract class AwakeSystem<T,A> : IAwkeSystem<A> where T : IAwake<A>
    {
        public void Run(object o,A a)
        {
            this.Awake((T)o,a);
        }

        public abstract void Awake(T self,A a);
       

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
            return typeof(IAwkeSystem<A>);

        }
        
        public Byte SystemPriority
        {
            get;
            set;
        }

    }
    
    /// <summary>
    /// T是Component/Entity,A是Parameter
    /// </summary>
    [ObjectSystem]

    public abstract class AwakeSystem<T,A,B> : IAwkeSystem<A,B> where T : IAwake<A,B>
    {
        public void Run(object o,A a, B b)
        {
            this.Awake((T)o,a,b);
        }

        public abstract void Awake(T self,A a, B b);
       

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
            return typeof(IAwkeSystem<A,B>);

        }
        
        public Byte SystemPriority
        {
            get;
            set;
        }

    }
    
    /// <summary>
    /// T是Component/Entity,A是Parameter
    /// </summary>
    [ObjectSystem]

    public abstract class AwakeSystem<T,A,B,C> : IAwkeSystem<A,B,C> where T : IAwake<A,B,C>
    {
        public void Run(object o,A a, B b, C c)
        {
            this.Awake((T)o,a,b,c);
        }

        public abstract void Awake(T self,A a, B b, C c);
       

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
            return typeof(IAwkeSystem<A,B,C>);

        }
        
        public Byte SystemPriority
        {
            get;
            set;
        }

    }
    
    /// <summary>
    /// T是Component/Entity,A是Parameter
    /// </summary>
    [ObjectSystem]

    public abstract class AwakeSystem<T,A,B,C,D> : IAwkeSystem<A,B,C,D> where T : IAwake<A,B,C,D>
    {
        public void Run(object o,A a, B b, C c, D d)
        {
            this.Awake((T)o,a,b,c,d);
        }

        public abstract void Awake(T self,A a, B b, C c, D d);
       

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
            return typeof(IAwkeSystem<A,B,C,D>);

        }
        
        public Byte SystemPriority
        {
            get;
            set;
        }

    }
}