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
        
        /// <summary>
        /// 进入输入模式
        /// </summary>
        public class EventEnterInputMode : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new EventEnterInputMode();
                UniEvent.SendMessage(msg);
            }
        }
        
        /// <summary>
        /// 退出输入模式
        /// </summary>
        public class EventExitInputMode : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new EventExitInputMode();
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
        
        /// <summary>
        /// 新建鼓点数据处理
        /// </summary>
        public class EventCreateCrateDrumBeatData : IEventMessage
        {
            public DrumBeatData DrumBeatData;
            public DrumBeatSceneData DrumBeatSceneData;
            public DrumBeatUIData DrumBeatUIData;
            
            public static void SendEventMessage(DrumBeatData drumBeatData, DrumBeatSceneData drumBeatSceneData,DrumBeatUIData drumBeatUIData)
            {
                var msg = new EventCreateCrateDrumBeatData();
                msg.DrumBeatData = drumBeatData;
                msg.DrumBeatSceneData = drumBeatSceneData;
                msg.DrumBeatUIData = drumBeatUIData;
                UniEvent.SendMessage(msg);
            }

        }

        /// <summary>
        /// 更新鼓点
        /// </summary>
        public class EventUpdateDrumBeatData : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new EventUpdateDrumBeatData();
                UniEvent.SendMessage(msg);
            }
        }

        /// <summary>
        /// 上传歌曲
        /// </summary>
        public class EventUploadMusic : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new EventUploadMusic();
              
                UniEvent.SendMessage(msg);
            }
        }
        
        /// <summary>
        /// 上传歌曲完成，处理数据
        /// </summary>
        public class EventUploadMusicComplete : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new EventUploadMusicComplete();
              
                UniEvent.SendMessage(msg);
            }
        }
        
        /// <summary>
        /// 删除鼓点数据处理
        /// </summary>
        public class EventDeleteDrumBeatData : IEventMessage
        {
            public DrumBeatData DrumBeatData;
            public DrumBeatSceneData DrumBeatSceneData;
            public DrumBeatUIData DrumBeatUIData;
            
            public static void SendEventMessage(DrumBeatData drumBeatData, DrumBeatSceneData drumBeatSceneData,DrumBeatUIData drumBeatUIData)
            {
                var msg = new EventDeleteDrumBeatData();
                msg.DrumBeatData = drumBeatData;
                msg.DrumBeatSceneData = drumBeatSceneData;
                msg.DrumBeatUIData = drumBeatUIData;
                UniEvent.SendMessage(msg);
            }

        }
        
        /// <summary>
        /// 查看鼓点信息
        /// </summary>
        public class EventQueryDrumBeatInfo : IEventMessage
        {
            public DrumBeatUIData DrumBeatUIData;
            public static void SendEventMessage(DrumBeatUIData drumBeatUIData)
            {
                var msg = new EventQueryDrumBeatInfo();
                msg.DrumBeatUIData = drumBeatUIData;
              
                UniEvent.SendMessage(msg);
            }
        }

        #endregion
        
        #region SaveLoad
        
        /// <summary>
        /// 开始保存关卡
        /// </summary>
        public class EventSaveLevelData : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new EventSaveLevelData();
              
                UniEvent.SendMessage(msg);
            }
        }
        
        /// <summary>
        /// 开始加载关卡
        /// </summary>
        public class EventLoadingLevelData : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new EventLoadingLevelData();
              
                UniEvent.SendMessage(msg);
            }
        }
        
        /// <summary>
        /// 关卡加载完成
        /// </summary>
        public class EventLoadedLevelData : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new EventLoadedLevelData();
              
                UniEvent.SendMessage(msg);
            }
        }
        
        #endregion


        #region Demo试玩模式输入事件

        public class EventDemoPoint1 : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new EventDemoPoint1();
              
                UniEvent.SendMessage(msg);
            }
        }
        
        
        public class EventDemoPoint2 : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new EventDemoPoint2();
              
                UniEvent.SendMessage(msg);
            }
        }

        #endregion
    }
    

  
}

