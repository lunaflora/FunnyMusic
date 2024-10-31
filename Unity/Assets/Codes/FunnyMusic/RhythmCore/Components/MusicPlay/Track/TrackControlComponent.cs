using System.Collections.Generic;
using FLib;
using Framework;
using RhythmEditor;
using UniFramework.Event;
using UnityEngine;

namespace FunnyMusic
{
    
    /// <summary>
    /// 音符/鼓点轨道移动控制
    /// </summary>
    [ChildType(typeof(DrumBeat))]
    public class TrackControlComponent : Entity,IAwake<GameObject,TrackType>,IUpdate,IDestroy
    {
        public GameObject TrackControlGameObject;
        public UITrackObject UITrackObject;
        public TrackType TrackType;

        /// <summary>
        /// 鼓点开始位置，判定位置
        /// </summary>
        public Transform DecisionAppearPoint, DecisionTipPoint;

        /// <summary>
        /// 轨道鼓点元素的根节点
        /// </summary>
        public Transform DrumBeatsRoot;

        public List<DrumBeatData> DrumBeatDatas = new List<DrumBeatData>();
        public int BeatIndex = 0;
        public List<long> ActiveBeats; 
        public long CurrentBeat =>  ActiveBeats == null || ActiveBeats.Count == 0 ? -1 : ActiveBeats[0];
        
        public readonly EventGroup EventGroup = new EventGroup();


    }
    
    
    [ObjectSystem]
    public class TrackControlComponentAwakeSystem : AwakeSystem<TrackControlComponent,GameObject,TrackType>
    {
        public override void Awake(TrackControlComponent self ,GameObject trackControlGameObject, TrackType trackType)
        {
            self.TrackType = trackType;
            self.Initialize(trackControlGameObject);
            self.AddEvent();
            
        }
    }
    
    [ObjectSystem]
    public class TrackControlComponentUpdateSystem : UpdateSystem<TrackControlComponent>
    {
        public override void Update(TrackControlComponent self)
        {
            self.MixerProcessFrame();
            
        }
    }
    
    [ObjectSystem]
    public class TrackControlComponentDestroySystem : DestroySystem<TrackControlComponent>
    {
        public override void Destroy(TrackControlComponent self)
        {
            self.RemoveEvent();
            
        }
    }

    public static class TrackControlComponentSystem
    {
        public static void Initialize(this TrackControlComponent self,GameObject trackControlGameObject)
        {
            self.TrackControlGameObject = trackControlGameObject;
            self.UITrackObject = self.TrackControlGameObject.GetComponent<UITrackObject>();
            self.DecisionAppearPoint = self.TrackControlGameObject.transform.Find("DecisionAppearPoint");
            self.DecisionTipPoint = self.TrackControlGameObject.transform.Find("DecisionTipPoint");

        }

        /// <summary>
        /// 处理每一帧，创建/删除 鼓点
        /// </summary>
        /// <param name="self"></param>
        public static void MixerProcessFrame(this TrackControlComponent self)
        {
            MusicPlayComponent musicPlayComponent = self.GetParent<MusicPlayComponent>();
            MusicPlaySetting musicPlaySetting = musicPlayComponent.MusicPlaySetting;
            
            if (self.BeatIndex == self.DrumBeatDatas.Count)
            {
                //鼓点播放完毕
                //FDebug.Print($"MixerProcessFrame 鼓点播放完毕 TrackType {self.TrackType}");
                return;
            }

            for (int i = self.BeatIndex; i < self.DrumBeatDatas.Count; i++)
            {
                //到时间创建鼓点
                float spawnTime = self.DrumBeatDatas[i].BeatTime + musicPlaySetting.LatencyCompensation -
                                  musicPlaySetting.SpawnTimeRange.x;

                if (musicPlayComponent.CurrentPlayTime >= spawnTime)
                {
                    self.AddChild<DrumBeat, DrumBeatData>(self.DrumBeatDatas[i]);
                    FDebug.Print($"创建鼓点 {self.DrumBeatDatas[i].ID}  {spawnTime}");
                    self.BeatIndex++;
                }
                else
                {
                    break;
                }
            }
            
            
            self.UpdateActiveBeats();

        }

        private static void UpdateActiveBeats(this TrackControlComponent self)
        {
            
        }

        public static Vector3 GetBeatDirection(this TrackControlComponent self)
        {
            return Vector3.Normalize(self.DecisionTipPoint.transform.position -
                                     self.DecisionAppearPoint.transform.position);

        }
        public static void SetActiveBeat(this TrackControlComponent self,long beatID)
        {
            if(self.ActiveBeats == null){ self.ActiveBeats  = new List<long>(); }
            self.ActiveBeats.Add(beatID);

           
        }

        public static void RemoveActiveBeat(this TrackControlComponent self,long beat)
        {
            if(self.ActiveBeats == null || self.ActiveBeats.Contains(beat) == false){return;}
            
            self.ActiveBeats.Remove(beat);
          
        }

        #region Event

        public static void AddEvent(this TrackControlComponent self)
        {
            self.EventGroup.AddListener<EventDefine.EventTriggerTrack>(self.TriggerDrumBeat);
        }
        
        public static void RemoveEvent(this TrackControlComponent self)
        {
            self.EventGroup.RemoveAllListener();
        }

        /// <summary>
        /// 通过点击/触屏 触发鼓点交互
        /// </summary>
        public static void TriggerDrumBeat(this TrackControlComponent self, IEventMessage eventMessage)
        {
            EventDefine.EventTriggerTrack eventTriggerTrack = eventMessage as EventDefine.EventTriggerTrack;
            if (self.TrackType != (TrackType)eventTriggerTrack.InputEventData.TrackID)
            {
                return;
            }

            DrumBeat drumBeat = self.GetChildAllowNone<DrumBeat>(self.CurrentBeat);
            if (drumBeat == null)
            {
                FDebug.Print($"找不到当前的鼓点 {self.ActiveBeats.Count}");
                return;
            }

            switch (drumBeat.BeatType)
            {
                case BeatType.Tap:
                    drumBeat.GetComponent<TapDrumBeatComponent>().TriggerDrumBeat();
                    break;
                
            }


        }

        #endregion
    }

}