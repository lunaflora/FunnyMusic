using System;
using CatJson;
using Cysharp.Threading.Tasks;
using Framework;
using UnityEditor;
using UnityEngine;

namespace FunnyMusic
{
    public static class ConfigHelper
    {
        [Obsolete]
        public async static UniTask<string> GetConfigText(string key)
        {
            string configText = "";
#if UNITY_EDITOR

            var url = $"{ResourcesPath.InternalConfigPath}{key}.txt";
            configText = AssetDatabase.LoadAssetAtPath<TextAsset>(url).text;
            //假装编辑器加载也是一个异步操作，统一接口
            await UniTask.Delay(TimeSpan.FromMilliseconds(100));
#else
            
#endif
            

            return configText;
        }
        
        public static JsonObject ToObject(string str)
        {
            return JsonHelper.FromJson(str);
        }
    }
}