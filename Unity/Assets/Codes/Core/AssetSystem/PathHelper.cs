using UnityEngine;

namespace Core
{
    public static class PathHelper
    {
        /// <summary>
        ///应用程序外部资源路径存放路径(热更新资源路径)
        /// </summary>
        public static string AppHotfixResPath
        {
            get
            {
                string game = Application.productName;
                string path = AppResPath;
                if (Application.isMobilePlatform)
                {
                    path = $"{Application.persistentDataPath}/{game}/";
                }
                return path;
            }
        }
        


        /// <summary>
        /// 应用程序内部资源路径存放路径
        /// </summary>
        public static string AppResPath
        {
            get
            {
                return Application.streamingAssetsPath;
            }
        }

        /// <summary>
        /// 应用程序内部资源路径存放路径(www/webrequest专用)
        /// </summary>
        public static string AppResPath4Web
        {
            get
            {
#if UNITY_IOS || UNITY_STANDALONE_OSX
                return $"file://{Application.streamingAssetsPath}";
#else
                return Application.streamingAssetsPath;
#endif

            }
        }

        #region Constant Path

        public static string BUNDLE_ROOT_PATH = "Assets/Bundles/";
        public static string BUNDLE_CONFIG_DEFALT_PATH = $"{BUNDLE_ROOT_PATH}Cfgs/default.bytes";
        public static string BUNDLE_AOTDLL_PATH = $"{BUNDLE_ROOT_PATH}Code/AotDlls";  
        public static string BUNDLE_HOTFIXDLL_PATH = $"{BUNDLE_ROOT_PATH}Code/HotfixDlls";
        
        public static string BUNDLE_LOGIC_PATH = $"{BUNDLE_ROOT_PATH}Logic/";
        public static string BUNDLE_GAMEOBJECTPOOL_PATH = $"{BUNDLE_LOGIC_PATH}GameObjectPool.prefab";
        
        public static string BUNDLE_SHADER_PATH = $"{BUNDLE_ROOT_PATH}Shaders/";
        
        //public static string BUNDLE_SHADER_PATH = $"{BUNDLE_ROOT_PATH}Shaders/";

        #endregion

    }
}