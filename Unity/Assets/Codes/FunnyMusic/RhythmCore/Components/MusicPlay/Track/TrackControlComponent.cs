using System.Collections.Generic;
using Framework;
using RhythmEditor;
using UnityEngine;

namespace FunnyMusic
{
    
    /// <summary>
    /// 音符/鼓点轨道移动控制
    /// </summary>
    [ChildType(typeof(DrumBeatComponent))]
    public class TrackControlComponent : Entity,IAwake<GameObject,TrackType>
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

    public static class TrackControlComponentSystem
    {
        public static void Initialize(this TrackControlComponent self,GameObject trackControlGameObject)
        {
            self.TrackControlGameObject = trackControlGameObject;
            self.DecisionAppearPoint = self.TrackControlGameObject.transform.Find("DecisionAppearPoint");
            self.DecisionTipPoint = self.TrackControlGameObject.transform.Find("DecisionTipPoint");

        }
    }

}