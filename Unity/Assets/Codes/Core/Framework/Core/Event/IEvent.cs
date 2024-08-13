using System;
using Cysharp.Threading.Tasks;

namespace Framework
{
    public interface IEvent
    {
        Type GetEventType();
    }

    public interface IEventClass : IEvent
    {

        void Handle(object o);

    }


    /// <summary>
    /// 原则上所有的事件接收者都必须继承AEventClass或者AEvent
    /// o为自定义class参数,class需要继承DisposeObject
    /// 主要为了消除gc
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Event]
    public abstract class AEventClass<T> : IEventClass where T : class
    {
        public Type GetEventType()
        {
            return typeof(T);
        }

        public void Handle(object o)
        {
            try
            {
                Excute(o);
            }
            catch(Exception e)
            {
                CustomLogger.Log(LoggerLevel.Error,e.ToString());
            }
        }

        protected abstract void Excute(object o);
    }
    
    /// <summary>
    /// 原则上所有的事件接收者都必须继承AEventClass或者AEvent
    /// o为自定义struct参数，一般同步事件都用这个
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Event]
    public abstract class AEvent<T> : IEvent where T : struct
    {
        public Type GetEventType()
        {
            return typeof(T);
        }

        public void Handle(T o)
        {
            try
            {
                Excute(o);
            }
            catch(Exception e)
            {
                CustomLogger.Log(LoggerLevel.Error,e.ToString());
            }
        }

        protected abstract void Excute(T o);
    }
    
    [Event]
    public abstract class AEventAsync<T> : IEvent where T : struct
    {
        public Type GetEventType()
        {
            return typeof(T);
        }

        public async UniTask Handle(T o)
        {
            try
            {
                await Excute(o);
            }
            catch(Exception e)
            {
                CustomLogger.Log(LoggerLevel.Error,e.ToString());
            }
        }

        protected abstract UniTask Excute(T o);
    }
}