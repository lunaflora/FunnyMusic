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

        /// <summary>
        /// 游玩鼓点列表
        /// </summary>
        public List<EditorPlayDrumBeat> PlayDrumBeats = new List<EditorPlayDrumBeat>();
        
        private readonly EventGroup eventGroup = new EventGroup();

        public bool InDemoMode = false;
        public int JudgeBeatIndex = -1;
        private List<DrumBeatData> DrumBeatDatas;

        private void Start()
        {
            DrumBeatDatas = EditorDataManager.Instance.DrumBeatDatas;
        }

        private void OnEnable()
        {
            eventGroup.AddListener<EditorEventDefine.EventCreateCrateDrumBeatData>(EventCreateCrateDrumBeatData);
            eventGroup.AddListener<EditorEventDefine.EventUpdateCurrentTime>(EventUpdateCurrentTime);
            eventGroup.AddListener<EditorEventDefine.EventSetCurrentTime>(EventSetCurrentTime);
            eventGroup.AddListener<EditorEventDefine.EventLoadedLevelData>(OnLoadLevel);
            eventGroup.AddListener<EditorEventDefine.EventDeleteDrumBeatData>(OnDeleteDrumBeatInfo);
            eventGroup.AddListener<EditorEventDefine.EventUpdateDrumBeatData>(OnUpdateDrumBeatData);
            
            
            eventGroup.AddListener<EditorEventDefine.EventEnterDemoMode>(OnEnterDemoMode);
            eventGroup.AddListener<EditorEventDefine.EventExitDemoMode>(OnExitDemoMode);
            eventGroup.AddListener<EditorEventDefine.EventExitInputMode>(OnExitInputMode);
            eventGroup.AddListener<EditorEventDefine.EventExitRecordMode>(OnExitRecordMode);
            
            eventGroup.AddListener<EditorEventDefine.EventDemoPoint1>(OnDemoPoint1);
            eventGroup.AddListener<EditorEventDefine.EventDemoPoint2>(OnDemoPoint2);
        }
        

        private void OnDisable()
        {
            eventGroup.RemoveAllListener();
        }


        #region Event
        
        private void OnDemoPoint2(IEventMessage eventMessage)
        {
            if(JudgeBeatIndex < 0)
                return;

            float currentTime = EditorDataManager.Instance.CurrentTime;
            DrumBeatData drumBeatData = DrumBeatDatas[JudgeBeatIndex];

            if (drumBeatData.BeatType != 1)
            {
                return;
            }

            float judgeTimeOffset =
                Mathf.Abs(currentTime + EditorDataManager.Instance.JudgeTimeCorrect - drumBeatData.BeatType);
            if (judgeTimeOffset < EditorDataManager.Instance.CoolValue)
            {
                EditorEventDefine.EventDemoDrumCool.SendEventMessage();
                if (JudgeBeatIndex + 1 < DrumBeatDatas.Count)
                {
                    JudgeBeatIndex++;
                }
                else
                {
                    JudgeBeatIndex = -1;
                }
            }

            if (judgeTimeOffset < EditorDataManager.Instance.GreatValue)
            {
                EditorEventDefine.EventDemoDrumGreat.SendEventMessage();
                if (JudgeBeatIndex + 1 < DrumBeatDatas.Count)
                {
                    JudgeBeatIndex++;
                }
                else
                {
                    JudgeBeatIndex = -1;
                }
            }
           
            if (judgeTimeOffset < EditorDataManager.Instance.BadValue)
            {
                EditorEventDefine.EventDemoDrumBad.SendEventMessage();
                if (JudgeBeatIndex + 1 < DrumBeatDatas.Count)
                {
                    JudgeBeatIndex++;
                }
                else
                {
                    JudgeBeatIndex = -1;
                }
            }
            
            if (judgeTimeOffset < EditorDataManager.Instance.MissValue)
            {
                EditorEventDefine.EventDemoDrumMiss.SendEventMessage();
                if (JudgeBeatIndex + 1 < DrumBeatDatas.Count)
                {
                    JudgeBeatIndex++;
                }
                else
                {
                    JudgeBeatIndex = -1;
                }
            }
        }

        private void OnDemoPoint1(IEventMessage eventMessage)
        {
           if(JudgeBeatIndex < 0)
               return;

           float currentTime = EditorDataManager.Instance.CurrentTime;
           DrumBeatData drumBeatData = DrumBeatDatas[JudgeBeatIndex];

           if (drumBeatData.BeatType != 0)
           {
               return;
           }

           float judgeTimeOffset =
               Mathf.Abs(currentTime + EditorDataManager.Instance.JudgeTimeCorrect - drumBeatData.BeatType);
           if (judgeTimeOffset < EditorDataManager.Instance.CoolValue)
           {
               EditorEventDefine.EventDemoDrumCool.SendEventMessage();
               if (JudgeBeatIndex + 1 < DrumBeatDatas.Count)
               {
                   JudgeBeatIndex++;
               }
               else
               {
                   JudgeBeatIndex = -1;
               }
           }

           if (judgeTimeOffset < EditorDataManager.Instance.GreatValue)
           {
               EditorEventDefine.EventDemoDrumGreat.SendEventMessage();
               if (JudgeBeatIndex + 1 < DrumBeatDatas.Count)
               {
                   JudgeBeatIndex++;
               }
               else
               {
                   JudgeBeatIndex = -1;
               }
           }
           
           if (judgeTimeOffset < EditorDataManager.Instance.BadValue)
           {
               EditorEventDefine.EventDemoDrumBad.SendEventMessage();
               if (JudgeBeatIndex + 1 < DrumBeatDatas.Count)
               {
                   JudgeBeatIndex++;
               }
               else
               {
                   JudgeBeatIndex = -1;
               }
           }
           if (judgeTimeOffset < EditorDataManager.Instance.MissValue)
           {
               EditorEventDefine.EventDemoDrumMiss.SendEventMessage();
               if (JudgeBeatIndex + 1 < DrumBeatDatas.Count)
               {
                   JudgeBeatIndex++;
               }
               else
               {
                   JudgeBeatIndex = -1;
               }
           }
        }
        
        private void OnExitRecordMode(IEventMessage eventMessage)
        {
            InDemoMode = false;
        }

        private void OnExitInputMode(IEventMessage eventMessage)
        {
            InDemoMode = false;
        }

        private void OnExitDemoMode(IEventMessage eventMessage)
        {
            InDemoMode = false;
        }

        private void OnEnterDemoMode(IEventMessage eventMessage)
        {
            InDemoMode = true;
            float currentTime = EditorDataManager.Instance.CurrentTime;
            JudgeBeatIndex = -1;
            JudgeBeatIndex = EditorDataManager.Instance.DrumBeatDatas.FindIndex(a => currentTime < a.BeatTime);
            
            
        }


        /// <summary>
        /// 打开关卡
        /// </summary>
        private void OnLoadLevel(IEventMessage eventMessage)
        {
            foreach (var drumBeat in PlayDrumBeats)
            {
                Destroy(drumBeat.gameObject);
            }
            PlayDrumBeats.Clear();

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

        private void EventCreateCrateDrumBeatData(IEventMessage message)
        {
            EditorEventDefine.EventCreateCrateDrumBeatData eventCreateCrateDrumBeatData =
                message as EditorEventDefine.EventCreateCrateDrumBeatData;

            CrateDrumBeatData(eventCreateCrateDrumBeatData.DrumBeatData, eventCreateCrateDrumBeatData.DrumBeatUIData,
                eventCreateCrateDrumBeatData.DrumBeatSceneData);

        }

        public void CrateDrumBeatData(DrumBeatData drumBeatData, DrumBeatUIData drumBeatUIData,DrumBeatSceneData drumBeatSceneData)
        {
            int beatType = drumBeatData.BeatType;
            drumBeatSceneData.Float_0 = BeatMoveTime;

            EditorPlayDrumBeat playDrumBeat =
                Instantiate(BeatPrefabList[beatType], BeatParent).GetComponent<EditorPlayDrumBeat>();
            PlayDrumBeats.Add(playDrumBeat);

            playDrumBeat.DrumBeatData = drumBeatData;
            playDrumBeat.DrumBeatSceneData = drumBeatSceneData;
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

            if (!InDemoMode)
            {
                return;
            }

            if (JudgeBeatIndex == -1)
            {
                return;
            }

            float currentTime = EditorDataManager.Instance.CurrentTime;
            DrumBeatData drumBeatData = DrumBeatDatas[JudgeBeatIndex];

            if (currentTime + EditorDataManager.Instance.JudgeTimeCorrect >
                drumBeatData.BeatTime + EditorDataManager.Instance.MissValue)
            {
                EditorEventDefine.EventDemoDrumMiss.SendEventMessage();
                if (JudgeBeatIndex + 1 < DrumBeatDatas.Count)
                {
                    JudgeBeatIndex++;
                }
                else
                {
                    JudgeBeatIndex = -1;
                }
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
        
           
        public void OnDeleteDrumBeatInfo(IEventMessage eventMessage)
        {
            EditorEventDefine.EventDeleteDrumBeatData deleteDrumBeatData =
                eventMessage as EditorEventDefine.EventDeleteDrumBeatData;

            EditorPlayDrumBeat playDrumBeat =
                PlayDrumBeats.Find(a => a.DrumBeatSceneData == deleteDrumBeatData.DrumBeatSceneData);

            PlayDrumBeats.Remove(playDrumBeat);
            DestroyImmediate(playDrumBeat.gameObject);
            

        }

        public void OnUpdateDrumBeatData(IEventMessage eventMessage)
        {
            for (int i = 0; i < PlayDrumBeats.Count; i++)
            {
                PlayDrumBeats[i].UpdateDrumBeat();
            }
        }


        #endregion
       
    }
}