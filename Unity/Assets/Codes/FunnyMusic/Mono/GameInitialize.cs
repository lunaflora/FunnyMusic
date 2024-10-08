using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Core;
using Cysharp.Threading.Tasks;
using FLib;
using Framework;
using UnityEngine;

namespace FunnyMusic
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
            InitCoreModule().Forget();

        }

       

        private void InitGameSingleton()
        {
            
        }

        async UniTask InitCoreModule()
        {
            //1.资源管理模块
            await AssetLoaderSystem.Instance.Initialize();
            
            //2.核心组件
            GameWorld.World.AddComponent<InputComponent>();
            
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
