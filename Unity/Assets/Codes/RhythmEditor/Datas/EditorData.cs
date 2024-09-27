using System;
using System.Runtime.InteropServices;

namespace RhythmEditor
{
    public enum SystemMode
    {
        EditorMode,
        DemoMode,
        RecordMode,
        InputMode
    }

    public enum LevelDifficulty
    {
        Easy,
        Normal,
        Hard,
        NightMare
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
        /// <summary>
        /// 鼓点显示序号
        /// </summary>
        public int Int_0;
        /// <summary>
        /// 鼓点UI轨道序列号
        /// </summary>
        public int Int_1;
        public int Int_2;
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
        /// <summary>
        /// 鼓点提示时间
        /// </summary>
        public float Float_0; 
        public float Float_1, Float_2;
        
    }
    
    /// <summary>
    /// 文件属性数据
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class FilePropertyData
    {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;
        public String filter = null;
        public String customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;
        public String file = null;
        public int maxFile = 0;
        public String fileTitle = null;
        public int maxFileTitle = 0;
        public String initialDir = null;
        public String title = null;
        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;
        public String defExt = null;
        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;
        public String templateName = null;
        public IntPtr reservedPtr = IntPtr.Zero;
        public int reservedInt = 0;
        public int flagsEx = 0;
    }
}

