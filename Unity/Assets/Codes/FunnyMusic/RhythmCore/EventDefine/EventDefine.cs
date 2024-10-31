using RhythmEditor;
using UniFramework.Event;

namespace FunnyMusic
{
    public class EventDefine
    {
        #region MusicPlay

        /// <summary>
        /// 点击轨道
        /// </summary>
        public class EventTriggerTrack : IEventMessage
        {
            public InputEventData InputEventData;
            public static void SendEventMessage(InputEventData inputEventData)
            {
                var msg = new EventDefine.EventTriggerTrack()
                {
                    InputEventData = inputEventData
                };
              
                UniEvent.SendMessage(msg);
            }
        }

        #endregion
        
    }
    
}