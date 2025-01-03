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
        public string RuntimeAudioPath;

        /// <summary>
        /// 当前音频播放的时间
        /// </summary>
        public float CurrentAudioTime = 0;
        /// <summary>
        /// 鼓点更新的实际线性时间
        /// </summary>
        public float CurrentPlayTime = 0;

        public List<DrumBeatData> DrumBeatDatas = new List<DrumBeatData>();
        public List<DrumBeatSceneData> DrumBeatSceneDatas = new List<DrumBeatSceneData>();
        public List<DrumBeatUIData> DrumBeatUIDatas = new List<DrumBeatUIData>();

        public LevelDifficulty LevelDifficulty = LevelDifficulty.Normal;
        
        #region Music Spped

        public float Bpm = 120;
        
        public float Crochet => 60f / Bpm;
        public float HalfCrochet => 30f / Bpm;
        public float QuarterCrochet => 15f / Bpm;
        

        #endregion
        
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


        #region 打击判定参数

        public float MissValue = 0.9f;
        public float BadValue = 0.6f;
        public float GreatValue = 0.4f;
        public float CoolValue = 0.2f;

        public float JudgeTimeCorrect = 0.25f;

        #endregion

        #region 音量控制

        public float MainVolume;
        public float BackGroundVolume;
        public float SongVolume;


        #endregion

    }
    
}

