using System;
using UnityEngine;

namespace Framework
{
    public static class Define
    {
        public const string BuildOutputDir = "./Temp/Bin/Debug";
        
#if UNITY_EDITOR && !ASYNC
        public static bool IsAsync = false;
#else
        public static bool IsAsync = true;
#endif
		
#if UNITY_EDITOR
        public static bool IsEditor = true;
#else
        public static bool IsEditor = false;
#endif


        public static UnityEngine.Object LoadAssetAtPath(string assetName,Type type)
        {
#if UNITY_EDITOR 
            return UnityEditor.AssetDatabase.LoadAssetAtPath(assetName,type);
#else
            return null;
#endif
        }
	
        public static string[] GetAssetPathsFromAssetBundle(string assetBundleName)
        {
#if UNITY_EDITOR 
            return UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
#else
            return new string[0];
#endif
        }
        
        public static string[] GetAssetPathFromAssetBundle(string assetBundleName,string assetName)
        {
#if UNITY_EDITOR
                return UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName,assetName);
#else
            return new string[0];
#endif
        }
	
        public static string[] GetAssetBundleDependencies(string assetBundleName, bool v)
        {
#if UNITY_EDITOR 
            return UnityEditor.AssetDatabase.GetAssetBundleDependencies(assetBundleName, v);
#else
			
           return new string[0];
#endif
        }
}
}