using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using FLib;
using UnityEngine;

namespace Game
{
    public class GameInitialize : MonoBehaviour
    {
        [Comment("FrameRate make sure the framerate is high enough on mobile")]
        public int forcedFrameRate = 60;
        
        private void Awake()
        {
            Application.targetFrameRate = forcedFrameRate;
            
            InitGameSingleton();
       
        }

       

        private void InitGameSingleton()
        {
            InputSystemHandler.Instance.Initialize();
        }

        async UniTask InitCoreModule()
        {
            await AssetLoaderSystem.Instance.Initialize();
            
        }
        
        #region GameMainLoop

        // Update is called once per frame
        private void Update()
        {
        
        }

        private void LateUpdate()
        {
            
        }

        private void OnApplicationQuit()
        {
            
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            
        }

        #endregion

    }

    
}
