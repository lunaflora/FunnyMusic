using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Core;
using Cysharp.Threading.Tasks;
using FLib;
using Framework;
using UnityEngine;

namespace Game
{
    public class GameInitialize : MonoBehaviour
    {
        [Comment("FrameRate make sure the framerate is high enough on mobile")]
        public int ForcedFrameRate = 60;
        
        private void Awake()
        {
            Application.targetFrameRate = ForcedFrameRate;
            GameWorld.Start(new List<Assembly>()
            {
                (typeof(GameInitialize).Assembly)
            },"MusicWorld");
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
            GameWorld.Update();
        
        }

        private void LateUpdate()
        {
            GameWorld.LateUpdate();
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
