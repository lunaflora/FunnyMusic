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

        #endregion

        public void RegisterEvents()
        {
            eventGroup.AddListener<EditorEventDefine.EventEnterDemoMode>(OnHandleEnterDemoMode);
            eventGroup.AddListener<EditorEventDefine.EventExitDemoMode>(OnHandleExitDemoMode);
            eventGroup.AddListener<EditorEventDefine.EventEnterRecordMode>(OnHandleEnterRecordMode);
            eventGroup.AddListener<EditorEventDefine.EventExitRecordMode>(OnHandleEventExitRecordMode);
       
        }

        private void OnHandleEnterDemoMode(IEventMessage message)
        {
            FDebug.Print("OnHandleEnterDemoMode");
        }
        
        private void OnHandleExitDemoMode(IEventMessage message)
        {
            FDebug.Print("OnHandleExitDemoMode");
        }
        private void OnHandleEnterRecordMode(IEventMessage message)
        {
            FDebug.Print("OnHandleEnterRecordMode");
        }
        private void OnHandleEventExitRecordMode(IEventMessage message)
        {
            FDebug.Print("OnHandleEventExitRecordMode");
        }
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
    }
}