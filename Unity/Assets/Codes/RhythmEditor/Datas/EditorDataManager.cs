using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace RhythmEditor
{
    public class EditorDataManager : MonoBehaviourSingleton<EditorDataManager>
    {
        [SerializeField]
        public SystemMode SystemMode = SystemMode.EditorMode;

        [SerializeField]
        public AudioClip LoadingAudio;

        public string LoadingAudioPath;

        /// <summary>
        /// </summary>
        public float CurrentTime = 0;

        public List<DrumBeatData> DrumBeatDatas = new List<DrumBeatData>();
        public List<DrumBeatSceneData> DrumBeatSceneDatas = new List<DrumBeatSceneData>();
        public List<DrumBeatUIData> DrumBeatUIDatas = new List<DrumBeatUIData>();

        
        /// <summary>
        /// 搜寻鼓点信息by ID
        /// </summary>
        public bool SearchDrumBeats(int id, out DrumBeatData drumBeatData, out DrumBeatUIData drumBeatUIData,
            out DrumBeatSceneData drumBeatSceneData)
        {
            int _id = id;
            drumBeatData = DrumBeatDatas.Find((data) => data.ID == _id);
            drumBeatUIData = DrumBeatUIDatas.Find((data) => data.ID == _id);
            drumBeatSceneData = DrumBeatSceneDatas.Find((data) => data.ID == _id);

            if (drumBeatData != null && drumBeatUIData != null && drumBeatSceneData != null)
            {
                return true;
            }
            else
            {
                if (drumBeatData != null)
                {
                    DrumBeatDatas.Remove(drumBeatData);
                }
                if (drumBeatUIData != null)
                {
                    DrumBeatUIDatas.Remove(drumBeatUIData);
                }
                if (drumBeatSceneData != null)
                {
                    DrumBeatSceneDatas.Remove(drumBeatSceneData);

                }
            }

            drumBeatData = null;
            drumBeatUIData = null;
            drumBeatSceneData = null;
            
            return false;

        }

    }
    
}

