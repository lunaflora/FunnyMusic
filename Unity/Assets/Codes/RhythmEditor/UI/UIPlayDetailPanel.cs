﻿using System;
using System.Collections.Generic;
using RhythmTool;
using TMPro;
using UniFramework.Event;
using UnityEngine;

namespace RhythmEditor
{
    /// <summary>
    /// 鼓点编辑面板
    /// </summary>
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
        public TextMeshProUGUI Bpm;

        #region MyRegion

        public RhythmData RhythmData;
        public RhythmAnalyzer RhythmAnalyzer;

        #endregion

        private void Start()
        {
            RhythmAnalyzer.Initialized += OnRhythmAnalyzeComplete;
        }

        private void OnEnable()
        {
            eventGroup.AddListener<EditorEventDefine.EventQueryDrumBeatInfo>(OnQueryDrumBeatInfo);
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
            Bpm.text = $"{EditorDataManager.Instance.Bpm}";
        }

        #region AnalyzeAudio 音频分析

        public void AnalyzeAudio()
        {
            if (EditorDataManager.Instance.LoadingAudio != null)
            { RhythmAnalyzer.Analyze(EditorDataManager.Instance.LoadingAudio);
               
            }
        }

        /// <summary>
        /// 添加Chroma特征到鼓点列表
        /// </summary>
        public void ApplyChroma()
        {
            
        }

        private void OnRhythmAnalyzeComplete(RhythmData rhythmData)
        {
            RhythmData = rhythmData;
            Track<Beat> track = rhythmData.GetTrack<Beat>();
            List<Beat> beatFeatures = new List<Beat>();
            track.GetFeatures(beatFeatures,1,5);
            float bpm = 120;
            foreach (var beat in beatFeatures)
            {
                bpm = Mathf.Round(beat.bpm * 10) / 10;
            }

            Bpm.text = $"{bpm}";
            EditorDataManager.Instance.Bpm = bpm;
        }

        #endregion

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
                ShowNoData();
                return;
            }

            DrumBeatID.text = DrumBeatData.ID.ToString();
            DrumBeaTime.text = DrumBeatData.BeatTime.ToString(".00");
            DrumBeaMoveTime.text = DrumBeatSceneData.Float_0.ToString(".00");
            
            

        }

        public void EditDrumBeatTime(string time)
        {
            Debug.Log(DrumBeaTime.text);
            if(string.IsNullOrEmpty(DrumBeaTime.text))
                return;
            float editTime = float.Parse(DrumBeaTime.text);
            if (editTime < 0)
            {
                return;
            }

            DrumBeatData.BeatTime = editTime;
            EditorDataManager.Instance.DrumBeatDatas.Sort(DrumBeatData.SortTime);
            EditorEventDefine.EventUpdateDrumBeatData.SendEventMessage();
        }
        
        /// <summary>
        /// 这里有个问题，收到的time为空
        /// </summary>
        /// <param name="time"></param>
        public void EditDrumBeatMoveTime(string time)
        {
            if(string.IsNullOrEmpty(DrumBeaMoveTime.text))
                return;
            float editTime = float.Parse(DrumBeaMoveTime.text);
            if (editTime < 0)
            {
                return;
            }
            
            DrumBeatSceneData.Float_0 = editTime;
            EditorEventDefine.EventUpdateDrumBeatData.SendEventMessage();
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


        public void DeleteAllDrumBeat()
        {
            
            DrumBeatData = null;
            DrumBeatUIData = null;
            DrumBeatSceneData = null;

            for (int i = 0; i < EditorDataManager.Instance.DrumBeatDatas.Count; i++)
            {
                var drumBeatData = EditorDataManager.Instance.DrumBeatDatas[i];
                var drumBeatUIData = EditorDataManager.Instance.DrumBeatUIDatas[i];
                var drumBeatSceneData = EditorDataManager.Instance.DrumBeatSceneDatas[i];
                EditorEventDefine.EventDeleteDrumBeatData.SendEventMessage(drumBeatData,drumBeatSceneData,drumBeatUIData);
            }
            EditorDataManager.Instance.DrumBeatDatas.Clear();
            EditorDataManager.Instance.DrumBeatUIDatas.Clear();
            EditorDataManager.Instance.DrumBeatSceneDatas.Clear();
            
            ShowNoData();
        }

        public void DeleteDrumBeat()
        {
            if (DrumBeatData == null)
            {
                return;
            }

            int nextIndex = DrumBeatUIData.Int_0;
            EditorDataManager.Instance.DrumBeatDatas.Remove(DrumBeatData);
            EditorDataManager.Instance.DrumBeatUIDatas.Remove(DrumBeatUIData);
            EditorDataManager.Instance.DrumBeatSceneDatas.Remove(DrumBeatSceneData);
            
            EditorEventDefine.EventDeleteDrumBeatData.SendEventMessage(DrumBeatData,DrumBeatSceneData,DrumBeatUIData);
            DrumBeatData = null;
            DrumBeatUIData = null;
            DrumBeatSceneData = null;

            List<DrumBeatData> drumBeatDatas = EditorDataManager.Instance.DrumBeatDatas;
            if (nextIndex >= drumBeatDatas.Count)
            {
                nextIndex--;
                if (nextIndex < 0)
                { 
                    ShowNoData();
                    return;
                }
            }

            EditorDataManager.Instance.SearchDrumBeats(drumBeatDatas[nextIndex].ID, out DrumBeatData,
                out DrumBeatUIData, out DrumBeatSceneData);
            
            EditorEventDefine.EventQueryDrumBeatInfo.SendEventMessage(DrumBeatUIData);
        }


        #endregion

        #region UIShow

        private void ShowNoData()
        {
            DrumBeatID.text = "Not Exist";
        }

        #endregion
        
    }
}