using System;
using System.Collections.Generic;
using UniFramework.Event;
using UnityEngine;

namespace RhythmEditor
{
    public class EditorPlayScene : MonoBehaviour
    {
        /// <summary>
        /// 鼓点移动时间
        /// </summary>
        [SerializeField]
        public float BeatMoveTime = 1.5f;
        
        /// <summary>
        /// 鼓点开始位置，结束位置，鼓点父级
        /// </summary>
        public Transform BeatStart,BeatEnd,BeatParent;
        public GameObject[] BeatPrefabList;

        public List<EditorPlayDrumBeat> PlayDrumBeats = new List<EditorPlayDrumBeat>();
        
        private readonly EventGroup eventGroup = new EventGroup();

        private void OnEnable()
        {
            eventGroup.AddListener<EditorEventDefine.EventCreateCrateDrumBeatData>(EventCreateCrateDrumBeatData);
            eventGroup.AddListener<EditorEventDefine.EventUpdateCurrentTime>(EventUpdateCurrentTime);
            eventGroup.AddListener<EditorEventDefine.EventSetCurrentTime>(EventSetCurrentTime);
        }

        private void OnDisable()
        {
            eventGroup.RemoveAllListener();
        }


        #region Event

        private void EventCreateCrateDrumBeatData(IEventMessage message)
        {
            EditorEventDefine.EventCreateCrateDrumBeatData eventCreateCrateDrumBeatData =
                message as EditorEventDefine.EventCreateCrateDrumBeatData;

            int beatType = eventCreateCrateDrumBeatData.DrumBeatData.BeatType;
            eventCreateCrateDrumBeatData.DrumBeatSceneData.Float_0 = BeatMoveTime;

            EditorPlayDrumBeat playDrumBeat =
                Instantiate(BeatPrefabList[beatType], BeatParent).GetComponent<EditorPlayDrumBeat>();
            PlayDrumBeats.Add(playDrumBeat);

            playDrumBeat.DrumBeatData = eventCreateCrateDrumBeatData.DrumBeatData;
            playDrumBeat.DrumBeatSceneData = eventCreateCrateDrumBeatData.DrumBeatSceneData;
            playDrumBeat.BeatStart = BeatStart;
            playDrumBeat.BeatEnd = BeatEnd;
            playDrumBeat.UpdateDrumBeat(0);

        }

        /// <summary>
        /// 自然更新Note位置
        /// </summary>
        /// <param name="message"></param>
        private void EventUpdateCurrentTime(IEventMessage message)
        {
            for (int i = 0; i < PlayDrumBeats.Count; i++)
            {
                PlayDrumBeats[i].UpdateDrumBeat();
            }
        }

        /// <summary>
        /// 手动设置时间时更新Note位置
        /// </summary>
        /// <param name="message"></param>
        private void EventSetCurrentTime(IEventMessage message)
        {
            float currentTime = (message as EditorEventDefine.EventSetCurrentTime).CurrentTime;

            for (int i = 0; i < PlayDrumBeats.Count; i++)
            {
                PlayDrumBeats[i].UpdateDrumBeat(currentTime);
            }
        }

        #endregion
       
    }
}