using System;
using Core;
using FLib;
using UniFramework.Event;

namespace RhythmEditor
{
    public class EditorEventSystem: Singleton<EditorEventSystem>
    {
        private readonly EventGroup eventGroup = new EventGroup();

        #region Callback

        public Action OnEnterDemoMode;
        public Action OnExitDemoMode;
        public Action OnEnterRecordMode;
        public Action OnExitRecordMode;

        public Action<int> OnCreateDrumBeat;

        #endregion

        public void RegisterEvents()
        {
            eventGroup.AddListener<EditorEventDefine.EventEnterDemoMode>(OnHandleEnterDemoMode);
            eventGroup.AddListener<EditorEventDefine.EventExitDemoMode>(OnHandleExitDemoMode);
            eventGroup.AddListener<EditorEventDefine.EventEnterRecordMode>(OnHandleEnterRecordMode);
            eventGroup.AddListener<EditorEventDefine.EventExitRecordMode>(OnHandleEventExitRecordMode);
            eventGroup.AddListener<EditorEventDefine.EventCrateDrumBeat>(OnHandleEventCreateDrumBeat);
       
        }

        
        private void OnHandleEnterDemoMode(IEventMessage message)
        {
            FDebug.Print("OnHandleEnterDemoMode");
            OnEnterDemoMode?.Invoke();
        }
        
        private void OnHandleExitDemoMode(IEventMessage message)
        {
            
            FDebug.Print("OnHandleExitDemoMode");
            OnExitDemoMode?.Invoke();
        }
        private void OnHandleEnterRecordMode(IEventMessage message)
        {
            FDebug.Print("OnHandleEnterRecordMode");
            OnEnterRecordMode?.Invoke();
        }
        private void OnHandleEventExitRecordMode(IEventMessage message)
        {
            FDebug.Print("OnHandleEventExitRecordMode");
            OnExitRecordMode.Invoke();
        }
        
        #region DrumBeats

        private void OnHandleEventCreateDrumBeat(IEventMessage message)
        {
            EditorEventDefine.EventCrateDrumBeat eventCrateDrumBeat = message as EditorEventDefine.EventCrateDrumBeat;
            FDebug.Print($"CreateDrumBeat {eventCrateDrumBeat.Index}");
            OnCreateDrumBeat?.Invoke(eventCrateDrumBeat.Index);
        }

        #endregion
    }
    
    public class EditorEventDefine
    {
        /// <summary>
        /// 进入试玩模式
        /// </summary>
        public class EventEnterDemoMode : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new EventEnterDemoMode();
                UniEvent.SendMessage(msg);
            }
        }
        /// <summary>
        /// 退出试玩模式
        /// </summary>
        public class EventExitDemoMode : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new EventExitDemoMode();
                UniEvent.SendMessage(msg);
            }
        }
        /// <summary>
        /// 进入录制模式
        /// </summary>
        public class EventEnterRecordMode : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new EventEnterRecordMode();
                UniEvent.SendMessage(msg);
            }
        }
        
        /// <summary>
        /// 退出录制模式
        /// </summary>
        public class EventExitRecordMode : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new EventExitRecordMode();
                UniEvent.SendMessage(msg);
            }
        }

        #region DrumBeats

        /// <summary>
        /// 新建鼓点
        /// </summary>
        public class EventCrateDrumBeat : IEventMessage
        {
            public int Index;
            public static void SendEventMessage(int index)
            {
                var msg = new EventCrateDrumBeat();
                msg.Index = index;
                UniEvent.SendMessage(msg);
            }
        }
        

        #endregion


        #region UI
        
        public class EventUpdateCurrentTime : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new EventUpdateCurrentTime();
                UniEvent.SendMessage(msg);
            }
        }
        
        public class EventSetCurrentTime : IEventMessage
        {
            public float CurrentTime = 0;
            public static void SendEventMessage(float currentTime)
            {
                var msg = new EventSetCurrentTime();
                msg.CurrentTime = currentTime;
                UniEvent.SendMessage(msg);
            }
        }

        #endregion
    }

  
}

