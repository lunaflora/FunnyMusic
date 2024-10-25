using System;
using System.Collections.Generic;
using FLib;
using UnityEngine;

namespace FunnyMusic
{
    /// <summary>
    /// 鼓点准确率配置
    /// </summary>
    [System.Serializable]
    public class BeatAccuracy
    {
        public string Name;
        //是否是Miss
        public bool Miss;
        //是否打断连击
        public bool BreakChain;
        //命中误差，越大容错越高
        public float PercentageTheshold;
        //分数
        public float Score;
        //命中Icon
        public Sprite Icon;
        //命中动画
        public GameObject popPrefab;
    }
    
    [System.Serializable]
    public class ScoreRank
    {
        public string Name;
        public Sprite Icon;
        public float PercentageTheshold;
    }
    
    [CreateAssetMenu(fileName = "MusicPlaySetting", menuName = "FunnyMusic/MusicPlaySetting", order = 0)]
    public class MusicPlaySetting : ScriptableObject
    {
        [SerializeField]
        public float BeatMoveTime = 1.5f;
        
        [SerializeField]
        public float BeatMoveSpeed = 0.5f;
        
        [Tooltip("音频播放的延迟时间")]
        [SerializeField] public float LatencyCompensation;
        
        [Tooltip("The x means the time the beat should be spawned before it reaches the target. " +
                 "The y means the time the beat should disappear after reaching target")]
        [SerializeField] public Vector2 SpawnTimeRange = new Vector2(9,3);
        
        
        [Tooltip("The accuracy table.")]
        [SerializeField] protected BeatAccuracy[] beatAccuracies;
        [Tooltip("The rank table.")]
        [SerializeField] protected ScoreRank[] scoreRanks;
        /// <summary>
        /// 排序后的列表
        /// </summary>
        protected Dictionary<string, BeatAccuracy> beatAccuracyDictionary;
        protected Dictionary<string, ScoreRank> scoreRankDictionary;

        protected float m_MaxNoteScore;

        public float MaxNoteScore => m_MaxNoteScore;
        
        public virtual IReadOnlyDictionary<string, BeatAccuracy> AccuracyDictionary
        {
            get
            {
                if (!m_Initialized) { Initialize(); }

                return beatAccuracyDictionary;
            }
        }
        public virtual IReadOnlyList<BeatAccuracy> OrderedAccuracyTable
        {
            get
            {
                if (!m_Initialized) { Initialize(); }

                return beatAccuracies;
            }
        }
        public virtual IReadOnlyDictionary<string, ScoreRank> RankDictionary
        {
            get
            {
                if (!m_Initialized) { Initialize(); }

                return scoreRankDictionary;
            }
        }
        public virtual IReadOnlyList<ScoreRank> OrderedRankTable
        {
            get
            {
                if (!m_Initialized) { Initialize(); }

                return scoreRanks;
            }
        }

        [NonSerialized] protected bool m_Initialized = false;

        public void Initialize()
        {
            //Beat Accuracy
            Array.Sort(beatAccuracies, new Comparison<BeatAccuracy>(
                (x, y) => x.Miss ? 1 : y.Miss ? -1 : x.PercentageTheshold.CompareTo(y.PercentageTheshold)));
            
            beatAccuracyDictionary = new Dictionary<string, BeatAccuracy>();
            for (int i = 0; i < beatAccuracies.Length; i++) {
                beatAccuracyDictionary.Add(beatAccuracies[i].Name, beatAccuracies[i]);
            }
        
            //Rank
            Array.Sort(scoreRanks, new Comparison<ScoreRank>( 
                (x, y) => x.PercentageTheshold.CompareTo(y.PercentageTheshold))); 
        
            scoreRankDictionary = new Dictionary<string, ScoreRank>();
            for (int i = 0; i < scoreRanks.Length; i++) {
                scoreRankDictionary.Add(scoreRanks[i].Name, scoreRanks[i]);
            }

            m_MaxNoteScore = GetBeatAccuracy(0).Score;

            m_Initialized = true;
        }
        
        /// <summary>
        /// 获取每次命中的准确率
        /// </summary>
        public virtual BeatAccuracy GetBeatAccuracy(float offsetPercentage)
        {
            BeatAccuracy last = null;
            
            for (int i = 0; i < beatAccuracies.Length; i++) {
                if (beatAccuracies[i].Miss) { continue; }
                
                if (offsetPercentage <= beatAccuracies[i].PercentageTheshold) {
                    FDebug.Print(offsetPercentage +" <= " +beatAccuracies[i].PercentageTheshold+" "+beatAccuracies[i].Name);
                    return beatAccuracies[i];
                }
                
                last = beatAccuracies[i];
            }

            return last;
        }
        
        /// <summary>
        /// 获取Miss的配置，主要用于Miss表现
        /// </summary>
        /// <returns></returns>
        public virtual BeatAccuracy GetMissAccuracy()
        {
            for (int i = 0; i < beatAccuracies.Length; i++) {
                if (beatAccuracies[i].Miss) { return beatAccuracies[i]; }
            }

            return null;
        }
        
        public virtual ScoreRank GetRank(float percentage)
        {
            ScoreRank scoreRank = scoreRanks[0];
            for (int i = 0; i < scoreRanks.Length; i++) {

                if (percentage > scoreRanks[i].PercentageTheshold) {
                    scoreRank = scoreRanks[i]; 
                    continue;
                }
                break;
            }

            return scoreRank;
        }

        public virtual int GetID(BeatAccuracy beatAccuracy)
        {
            return Array.IndexOf(beatAccuracies, beatAccuracy);
        }
    }
}