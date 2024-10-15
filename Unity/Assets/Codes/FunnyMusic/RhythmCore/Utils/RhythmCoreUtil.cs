namespace FunnyMusic
{
    public static class RhythmCoreUtil
    {
        #region 编辑时到运行时的转换

        /// <summary>
        /// 
        /// </summary>
        public static int BeatTypeToTrackID(int beatType)
        {
            return beatType + 1;
        }

        #endregion
    }
}