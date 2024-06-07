using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace Core
{
    [CreateAssetMenu(fileName = "AssetSetting", menuName = "Settings/AssetSetting", order = 1)]
    public class AssetSetting : ScriptableObject
    {
        /// <summary>
        /// 资源系统运行模式
        /// </summary>
        [SerializeField]
        public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

        public string PackageName = "MainPackage";
        
        
    }

}