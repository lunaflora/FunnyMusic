using System;

namespace RhythmEditor
{
    public enum SystemMode
    {
        EditorMode,
        DemoMode,
        RecordMode
    }

    /// <summary>
    /// 鼓点基础数据
    /// </summary>
    [Serializable]
    public class DrumBeatData
    {
        public int ID;
        /// <summary>
        /// 鼓点时间
        /// </summary>
        public float BeatTime;
        /// <summary>
        /// 鼓点类型
        /// </summary>
        public int BeatType;

        public static int SortTime(DrumBeatData a, DrumBeatData b)
        {
            if (a.BeatTime > b.BeatTime)
            {
                return 1;
            }
            else if(a.BeatTime == b.BeatTime)
            {
                if (a.ID > b.ID)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }

        public static int SortID(DrumBeatData a, DrumBeatData b)
        {
            if (a.ID > b.ID)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        
    }

    
    /// <summary>
    /// 鼓点UI数据
    /// </summary>
    [Serializable]
    public class DrumBeatUIData
    {
        public int ID;
        public int Int_0, Int_1, Int_2;
        public float Float_0, Float_1, Float_2;
    }
    
    
    /// <summary>
    /// 鼓点场景数据
    /// </summary>
    [Serializable]
    public class DrumBeatSceneData
    {
        public int ID;
        public int Int_0, Int_1, Int_2;
        public float Float_0, Float_1, Float_2;
        
    }
}

