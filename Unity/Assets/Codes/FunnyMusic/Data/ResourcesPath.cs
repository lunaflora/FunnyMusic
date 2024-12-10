namespace FunnyMusic
{
    public static class ResourcesPath
    {
        public static string InternalBundleRoot = "Assets/Bundles/";
        public static string InternalConfigPath = InternalBundleRoot + "Config/{0}.txt";

        #region MusicPrefab

        public static string InternalCoreBattlePath = $"{InternalBundleRoot}Battle/Prefabs/CoreBattle.prefab"; 
        public static string InternalMusicPlayPath = $"{InternalBundleRoot}MusicPlay/Prefabs/MusicPlay.prefab";

        public static string InternalDrumBeatPath = $"{InternalBundleRoot}MusicPlay/Prefabs/BeatGameObject/" + "{0}.prefab";

        #endregion

        #region CoreConfig

        public static string InternalLevelConfig = $"{InternalBundleRoot}Levels/"; 

        #endregion


        #region Unit

        public static string InternalUnitGameObjectPath = InternalBundleRoot + "Battle/Prefabs/Player/{0}/{1}.prefab";

        #endregion
    }
}