using System;
using TMPro;
using UniFramework.Event;
using UnityEngine;

namespace RhythmEditor
{
    public class UIPlayDetailPanel : MonoBehaviour
    {
        private readonly EventGroup eventGroup = new EventGroup();

        #region UI

        public TextMeshProUGUI DrumBeatID;
        public TMP_InputField DrumBeaTime;
        public TMP_InputField DrumBeaMoveTime;
        #endregion

        private DrumBeatData DrumBeatData;
        private DrumBeatUIData DrumBeatUIData;
        private DrumBeatSceneData DrumBeatSceneData;
      
        private void OnEnable()
        {
            eventGroup.AddListener<EditorEventDefine.EventQueryDrumBeatInfo>(OnQueryDrumBeatInfo);
            eventGroup.AddListener<EditorEventDefine.EventDeleteDrumBeatData>(OnDeleteDrumBeatInfo);
        }

        private void OnDisable()
        {
            eventGroup.RemoveAllListener();
        }

        public void OnQueryDrumBeatInfo(IEventMessage eventMessage)
        {
            EditorEventDefine.EventQueryDrumBeatInfo queryDrumBeatInfo =
                eventMessage as EditorEventDefine.EventQueryDrumBeatInfo;
          

            int ID = queryDrumBeatInfo.DrumBeatUIData.ID;
            bool isExit =
                EditorDataManager.Instance.SearchDrumBeats(ID, out DrumBeatData, out DrumBeatUIData,
                    out DrumBeatSceneData);
            if (!isExit)
            {
                DrumBeatID.text = "Not Exist";
                return;
            }

            DrumBeatID.text = DrumBeatData.ID.ToString();
            DrumBeaTime.text = DrumBeatData.BeatTime.ToString(".00");
            DrumBeaMoveTime.text = DrumBeatSceneData.Float_0.ToString(".00");
            
            

        }

        public void OnDeleteDrumBeatInfo(IEventMessage eventMessage)
        {
            
        }

        #region ButtonFunction

        public void JumpDrumBeatTime()
        {
            if (DrumBeatData == null)
            {
                return;
            }
            
            EditorEventDefine.EventSetCurrentTime.SendEventMessage(DrumBeatData.BeatTime);
        }

        public void DeleteDrumBeat()
        {
            if (DrumBeatData == null)
            {
                return;
            }
            
            
        }
        
        
        #endregion
        
    }
}