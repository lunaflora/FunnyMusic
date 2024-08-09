using Core;
using UniFramework.Event;

namespace RhythmEditor
{
    public class EditorEventSystem
    {
        
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