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
    public class TrackControlComponent : Entity,IAwake<GameObject,int>
    {
        public GameObject TrackControlGameObject;
        public int TrackIndex;

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
    public class TrackControlComponentAwakeSystem : AwakeSystem<TrackControlComponent,GameObject,int>
    {
        public override void Awake(TrackControlComponent self ,GameObject trackControlGameObject, int trackIndex)
        {
            self.TrackIndex = trackIndex;
            self.Initialize(trackControlGameObject);
        }
    }

    public static class TrackControlComponentSystem
    {
        public static void Initialize(this TrackControlComponent self,GameObject trackControlGameObject)
        {
            self.TrackControlGameObject = trackControlGameObject;

        }
    }

}