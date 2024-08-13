using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Framework
{
    public sealed class EventSystem : IDisposable
    {
        private static EventSystem instance;

        public static EventSystem Instance
        {

            get
            {
                if (instance == null)
                {
                    instance = new EventSystem();
                }

                return instance;
            }
        }

        private readonly Dictionary<Type, List<object>> allEvents = new Dictionary<Type, List<object>>();


        public void AddAll(List<Type> eventTypes)
        {
            this.allEvents.Clear();

            foreach (var type in eventTypes)
            {
                IEvent iEvent = Activator.CreateInstance(type) as IEvent;
                if (iEvent != null)
                {
                    //注意,这个是自定义的事件类型
                    Type eventType = iEvent.GetEventType();
                    if (!allEvents.ContainsKey(eventType))
                    {
                        this.allEvents.Add(eventType,new List<object>());
                    }
                    
                    allEvents[eventType].Add(iEvent);
                    
                }

            }

        
        }
        
        //异步事件
        public async UniTask TriggerAsync<T>(T a) where T : struct
        {
            List<object> iEvents;
            if (!this.allEvents.TryGetValue(a.GetType(), out iEvents))
            {
                return;
            }

            using (ListComponent<UniTask> list = ListComponent<UniTask>.Create())
            {
                for (int i = 0; i < iEvents.Count; i++)
                {
                    object obj = iEvents[i];
                    if(!(obj is AEventAsync<T> aEvent))
                    {
                    
                        CustomLogger.Log(LoggerLevel.Error,$"event has error {obj.GetType().Name}");
                        continue;
                    }
                    
                    list.Add(aEvent.Handle(a));
                }
                
                //开始等待所有异步事件执行完成

                await UniTask.WhenAll(list);
            }
            
            
        }


        //同步事件
        public void TriggerSync<T>(T a) where T : struct
        {
            List<object> iEvents;
            if (!this.allEvents.TryGetValue(a.GetType(), out iEvents))
            {
                return;
            }

            for (int i = 0; i < iEvents.Count; i++)
            {
                object obj = iEvents[i];
                if (!(obj is AEvent<T> aEvent))
                {
                    CustomLogger.Log(LoggerLevel.Error,$"event has error {obj.GetType().Name}");
                    continue;
                }
                
                aEvent.Handle(a);
            }
        }
        
        
        /// <summary>
        /// 消除gc时候使用
        /// </summary>
        /// <param name="a"></param>
        /// <typeparam name="T"></typeparam>
        public void TriggerClass<T>(T a) where T : DynamicObject
        {
            List<object> iEvents;
            if (!this.allEvents.TryGetValue(a.GetType(), out iEvents))
            {
                return;
            }

            for (int i = 0; i < iEvents.Count; i++)
            {
                object obj = iEvents[i];
                if (!(obj is AEventClass<T> aEvent))
                {
                    CustomLogger.Log(LoggerLevel.Error,$"event has error {obj.GetType().Name}");
                    continue;
                }
                
                aEvent.Handle(a);
            }
            
            a.Dispose();
        }

        public  void Dispose()
        {
            
            instance = null;

        }

    }
}