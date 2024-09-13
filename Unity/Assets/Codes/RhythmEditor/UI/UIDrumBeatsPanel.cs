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
            eventGroup.AddListener<EditorEventDefine.EventLoadedLevelData>(OnLoadLevel);
        }
        
        private void OnDisable()
        {
            eventGroup.RemoveAllListener();
        }


        /// <summary>
        /// 打开关卡
        /// </summary>
        private void OnLoadLevel(IEventMessage eventMessage)
        {
            for (int i = UIDrumBeatItems.Count -1; i >= 0; i--)
            {
                Destroy(UIDrumBeatItems[i].gameObject);
            }
            
            UIDrumBeatItems.Clear();
            TrackID = 0;

            int ID;
            List<DrumBeatData> drumBeatDatas = EditorDataManager.Instance.DrumBeatDatas;
            DrumBeatData drumBeatData;
            DrumBeatUIData drumBeatUIData;
            DrumBeatSceneData drumBeatSceneData;

            for (int i = 0; i < drumBeatDatas.Count; i++)
            {
                ID = drumBeatDatas[i].ID;
                bool isExist = EditorDataManager.Instance.SearchDrumBeats(ID, out drumBeatData, out drumBeatUIData,
                    out drumBeatSceneData);
                if (isExist)
                {
                    CrateDrumBeatData(drumBeatData,drumBeatUIData, drumBeatSceneData);
                }
            }

        }

        private void UploadMusicComplete(IEventMessage eventMessage)
        {
            Initialize();
            RefreshUIDrumBeatID();
        }


        private void EventCreateCrateDrumBeatData(IEventMessage message)
        {
            EditorEventDefine.EventCreateCrateDrumBeatData eventCreateCrateDrumBeatData =
                message as EditorEventDefine.EventCreateCrateDrumBeatData;

            CrateDrumBeatData(eventCreateCrateDrumBeatData.DrumBeatData, eventCreateCrateDrumBeatData.DrumBeatUIData,
                eventCreateCrateDrumBeatData.DrumBeatSceneData);



        }

        public void CrateDrumBeatData(DrumBeatData drumBeatData, DrumBeatUIData drumBeatUIData,
            DrumBeatSceneData drumBeatSceneData)
        {
            int beatType = drumBeatData.BeatType;
            UIDrumBeatItem beatItem = Instantiate(UIDrumBeatItem[beatType], WaveformUITracks[TrackID])
                .GetComponent<UIDrumBeatItem>();
            beatItem.DrumBeatUIData = drumBeatUIData;
        
            UIDrumBeatItems.Add(beatItem);
            RefreshUIDrumBeatID();
            drumBeatUIData.Int_1 = TrackID;
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