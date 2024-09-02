using System;
using System.Collections.Generic;
using UniFramework.Event;
using UnityEngine;

namespace RhythmEditor
{
    /// <summary>
    /// 鼓点界面
    /// </summary>
    public class UIDrumBeatsPanel : MonoBehaviour
    {
        public RectTransform[] WaveformUITracks;
        public GameObject[] UIDrumBeatItem;
        public List<UIDrumBeatItem> UIDrumBeatItems = new List<UIDrumBeatItem>();

        private readonly EventGroup eventGroup = new EventGroup();

        private int TrackID = 0;
        
        private void Initialize()
        {
            float audioLength = EditorDataManager.Instance.LoadingAudio.length;
            for (int i = 0; i < WaveformUITracks.Length; i++)
            {
                WaveformUITracks[i].sizeDelta = new Vector2(audioLength * UIConstValue.UIWidthScale, 50f);
            }
        }
        
        private void Start()
        {
            Initialize();
        }

        private void OnEnable()
        {
            eventGroup.AddListener<EditorEventDefine.EventCreateCrateDrumBeatData>(EventCreateCrateDrumBeatData);
            eventGroup.AddListener<EditorEventDefine.EventUploadMusicComplete>(UploadMusicComplete);
        }
        
        private void UploadMusicComplete(IEventMessage eventMessage)
        {
            Initialize();
        }

        private void OnDisable()
        {
            eventGroup.RemoveAllListener();
        }

        private void EventCreateCrateDrumBeatData(IEventMessage message)
        {
            EditorEventDefine.EventCreateCrateDrumBeatData eventCreateCrateDrumBeatData =
                message as EditorEventDefine.EventCreateCrateDrumBeatData;

            int beatType = eventCreateCrateDrumBeatData.DrumBeatData.BeatType;
            UIDrumBeatItem beatItem = Instantiate(UIDrumBeatItem[beatType], WaveformUITracks[TrackID])
                .GetComponent<UIDrumBeatItem>();
            beatItem.DrumBeatUIData = eventCreateCrateDrumBeatData.DrumBeatUIData;
        
            UIDrumBeatItems.Add(beatItem);
            RefreshUIDrumBeatID();
            eventCreateCrateDrumBeatData.DrumBeatUIData.Int_1 = TrackID;
            TrackID++;
            TrackID = TrackID >= WaveformUITracks.Length ? 0 : TrackID;
            
        }

        private void RefreshUIDrumBeatID()
        {
            for (int i = 0; i < UIDrumBeatItems.Count; i++)
            {
                UIDrumBeatItems[i].DrumBeatUIData.Int_0 =
                    EditorDataManager.Instance.DrumBeatDatas.FindIndex(
                        a => a.ID == UIDrumBeatItems[i].DrumBeatUIData.ID);
                
                UIDrumBeatItems[i].UpdateDrumBeatItem();
            }
        }

    }
}