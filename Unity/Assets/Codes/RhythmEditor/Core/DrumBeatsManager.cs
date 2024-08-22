using System;
using System.Collections.Generic;
using ParadoxNotion;
using UnityEngine;

namespace RhythmEditor
{
    public class DrumBeatsManager : MonoBehaviour
    {
        private List<DrumBeatData> DrumBeatDatas = new List<DrumBeatData>();
        private List<DrumBeatUIData> DrumBeatUIDatas = new List<DrumBeatUIData>();
        private List<DrumBeatSceneData> DrumBeatSceneDatas = new List<DrumBeatSceneData>();

        private void Start()
        {
            DrumBeatDatas = EditorDataManager.Instance.DrumBeatDatas;
            DrumBeatUIDatas = EditorDataManager.Instance.DrumBeatUIDatas;
            DrumBeatSceneDatas = EditorDataManager.Instance.DrumBeatSceneDatas;
        }

        private void OnDisable()
        {
            EditorEventSystem.Instance.OnCreateDrumBeat -= OnCreateDrumBeat;
        }

        private void OnEnable()
        {
            EditorEventSystem.Instance.OnCreateDrumBeat += OnCreateDrumBeat;
        }

        /// <summary>
        /// 创建新的鼓点
        /// </summary>
        /// <param name="index"></param>
        private void OnCreateDrumBeat(int index)
        {
            DrumBeatData drumBeatData = new DrumBeatData();
            DrumBeatUIData drumBeatUIData = new DrumBeatUIData();
            DrumBeatSceneData drumBeatSceneData = new DrumBeatSceneData();

            int newID = FreeDrumBeatID();
            drumBeatData.ID = newID;
            drumBeatData.BeatTime = EditorDataManager.Instance.CurrentTime;
            drumBeatData.BeatType = index;

            drumBeatUIData.ID = newID;
            drumBeatSceneData.ID = newID;
            
            DrumBeatDatas.Add(drumBeatData);
            DrumBeatDatas.Sort(DrumBeatData.SortTime);
            DrumBeatSceneDatas.Add(drumBeatSceneData);
            DrumBeatUIDatas.Add(drumBeatUIData);

        }

        private int FreeDrumBeatID()
        {
            int newID = 1001;
            DrumBeatDatas.Sort(DrumBeatData.SortID);

            for (int i = 0; i < DrumBeatDatas.Count; i++)
            {
                if (DrumBeatDatas[i].ID == newID)
                {
                    newID++;
                }
                else
                {
                    break;
                }
            }
            
            DrumBeatDatas.Sort(DrumBeatData.SortTime);

            return newID;
        }
    }
}