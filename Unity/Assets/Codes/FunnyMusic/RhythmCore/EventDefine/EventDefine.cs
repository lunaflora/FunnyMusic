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
            public int TrackID;
            public static void SendEventMessage(int trackID)
            {
                var msg = new EventDefine.EventTriggerTrack()
                {
                    
                };
              
                UniEvent.SendMessage(msg);
            }
        }

        #endregion
        
    }
    
}