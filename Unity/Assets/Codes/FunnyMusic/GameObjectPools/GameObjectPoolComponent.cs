using System;
using Core;
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;

namespace FunnyMusic
{
    public class GameObjectPoolComponent : MonoBehaviour
    {

        private GameObjectPoolSetting gameObjectPoolSetting;

        private void Awake()
        {
            gameObjectPoolSetting =
                AssetLoaderSystem.Instance.LoadAssetSync<GameObjectPoolSetting>(WordPathHelper.BUNDLE_POOL_SETTING);
            
            
            GameObjectPoolManager.Root = transform;
            
            GameObjectPoolManager.DefaultObjectExpireTime = gameObjectPoolSetting.DefaultObjectExpireTime;
            GameObjectPoolManager.DefaultPoolExpireTime = gameObjectPoolSetting.DefaultPoolExpireTime;
            GameObjectPoolManager.MaxInstantiateCount = gameObjectPoolSetting.MaxInstantiateCount;
        }

        private GameObject testUnit;
        private void Update()
        { 
            
            GameObjectPoolManager.Update(Time.deltaTime);
            

            
        }

        #region Test
        

        #endregion
    }
}