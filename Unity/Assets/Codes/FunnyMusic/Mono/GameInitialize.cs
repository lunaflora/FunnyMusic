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
            GameWorld.World.AddComponent<ConfigGenerateComponent>();
            await ConfigGenerateComponent.Instance.LoadAllAsync();
            ConfigManager.Instance.Initialize();
            
            GameWorld.World.AddComponent<InputComponent>();
            GameWorld.World.AddComponent<GlobalGameObjectComponent>();
            GameWorld.World.AddComponent<TimerComponent>();
            GameWorld.World.AddComponent<UnitComponent>();
            
            //3.游戏组件
            await InitGameModule();
            
            //4.进入测试关卡
            EnterLevel();

        }

        async UniTask InitGameModule()
        {

            var BattleCore = await AssetLoaderSystem.Instance.InstantiateSync(ResourcesPath.InternalCoreBattlePath,
                GlobalGameObjectComponent.Instance.Controller);

        }

        private void EnterLevel()
        {
            GameWorld.World.AddComponent<MusicPlayComponent, string>("level_001");
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
