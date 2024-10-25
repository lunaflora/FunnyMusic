using System.Collections.Generic;
using FLib;
using Framework;
using RhythmEditor;
using UnityEngine;

namespace FunnyMusic
{
    
    /// <summary>
    /// 音符/鼓点轨道移动控制
    /// </summary>
    [ChildType(typeof(DrumBeatComponent))]
    public class TrackControlComponent : Entity,IAwake<GameObject,TrackType>,IUpdate
    {
        public GameObject TrackControlGameObject;
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
        

    }
    
    
    [ObjectSystem]
    public class TrackControlComponentAwakeSystem : AwakeSystem<TrackControlComponent,GameObject,TrackType>
    {
        public override void Awake(TrackControlComponent self ,GameObject trackControlGameObject, TrackType trackType)
        {
            self.TrackType = trackType;
            self.Initialize(trackControlGameObject);
            
        }
    }
    
    [ObjectSystem]
    public class TrackControlComponentUodateSystem : UpdateSystem<TrackControlComponent>
    {
        public override void Update(TrackControlComponent self)
        {
            self.MixerProcessFrame();
            
        }
    }

    public static class TrackControlComponentSystem
    {
        public static void Initialize(this TrackControlComponent self,GameObject trackControlGameObject)
        {
            self.TrackControlGameObject = trackControlGameObject;
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
                    self.AddChild<DrumBeatComponent, DrumBeatData>(self.DrumBeatDatas[i]);
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
    }

}