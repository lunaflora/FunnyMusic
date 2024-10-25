using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using FLib;
using Framework;
using RhythmEditor;
using UnityEngine;

namespace FunnyMusic
{
    /// <summary>
    /// 节奏音乐核心逻辑组件
    /// </summary>
    [ChildType(typeof(TrackControlComponent))]
    public class MusicPlayComponent : Entity,IAwake<string>,IDestroy,IUpdate
    {
        /// <summary>
        /// 主控制器
        /// </summary>
        public GameObject MusicPlay;
        /// <summary>
        /// 主音乐
        /// </summary>
        public AudioSource PlayMusicSource;
        /// <summary>
        /// 当前音频播放的时间
        /// </summary>
        public float CurrentAudioTime = 0;
        /// <summary>
        /// 鼓点更新的实际线性时间
        /// </summary>
        public float CurrentPlayTime = 0;

        public MusicPlayState MusicPlayState;

        public MusicPlaySetting MusicPlaySetting = null;

    }
    
    [ObjectSystem]
    public class MusicPlayComponentAwakeSystem : AwakeSystem<MusicPlayComponent,string>
    {
        public override void Awake(MusicPlayComponent self ,string levelName)
        {
            self.Initialize(levelName).Forget();
        }
    }
    
    [ObjectSystem]
    public class MusicPlayComponentUpdateSystem : UpdateSystem<MusicPlayComponent>
    {
        public override void Update(MusicPlayComponent self)
        {
            
            self.Update();
       
        }
    }
    
    [ObjectSystem]
    public class MusicPlayComponentDestroySystem : DestroySystem<MusicPlayComponent>
    {
        public override void  Destroy(MusicPlayComponent self)
        {
            
            self.Destroy();
       
        }
    }
    
    public static class MusicPlayComponentSystem
    {

        #region Initialize

        public static async UniTask Initialize(this MusicPlayComponent self,string levelName)
        {
            self.MusicPlayState = MusicPlayState.Prepare;
            //1.加载MusicPlay预制
            self.MusicPlay = await AssetLoaderSystem.Instance.InstantiateSync(ResourcesPath.InternalMusicPlayPath,
                GlobalGameObjectComponent.Instance.Controller);
            
            //2.加载MusicPlayData
            self.MusicPlaySetting =
                await AssetLoaderSystem.Instance.LoadAssetAsync<MusicPlaySetting>(
                    $"{ResourcesPath.InternalLevelConfig}{levelName}_play.asset");
            
            //3.加载制谱表
            await self.LoadMusicLevel(levelName);

            
            
            FDebug.Print($"MusicPlaySetting {self.MusicPlaySetting.BeatMoveTime}");


        }

        /// <summary>
        /// 加载音乐关卡谱面，并解析为运行时数据
        /// </summary>
        public static async UniTask LoadMusicLevel(this MusicPlayComponent self,string levelName)
        {
            var levelConfig = await AssetLoaderSystem.Instance.LoadAssetAsync<TextAsset>(
                $"{ResourcesPath.InternalLevelConfig}{levelName}.txt");
            
            JsonUtility.FromJsonOverwrite(levelConfig.text, EditorDataManager.Instance);

            self.PlayMusicSource = self.MusicPlay.GetComponent<AudioSource>();

            string audioPath = FileHelper.FilePathToUnity(EditorDataManager.Instance.LoadingAudioPath);
            AudioClip audioClip =  await AssetLoaderSystem.Instance.LoadAssetAsync<AudioClip>(audioPath);
            self.PlayMusicSource.clip = audioClip;
            
           
            self.InitTrackComponent();

            await self.PlayMusic();

        }

        /// <summary>
        /// 挂载轨道组件
        /// </summary>
        private static void InitTrackComponent(this MusicPlayComponent self)
        {
            for (int i = 0; i < (int)TrackType.TrackCount; i++)
            {
                GameObject trackObject = self.MusicPlay.transform.Find($"PlayArea/AudioTracks/Track{i +1}").gameObject;
                var trackControlComponent = self.AddChild<TrackControlComponent,GameObject,TrackType>(trackObject, (TrackType)i);
                trackControlComponent.DrumBeatDatas =
                    EditorDataManager.Instance.DrumBeatDatas.FindAll((beat) => beat.BeatType == i);
                //按时间先后排序
                trackControlComponent.DrumBeatDatas.Sort(DrumBeatData.SortTime);
            }
        }

        private static async UniTask PlayMusic(this MusicPlayComponent self)
        {
            await TimerComponent.Instance.WaitAsync((long)self.MusicPlaySetting.LatencyCompensation * 1000);
            self.PlayMusicSource.Play();
            self.CurrentAudioTime = self.PlayMusicSource.time;
            self.MusicPlayState = MusicPlayState.Play;
        }


        #endregion
        

        #region Update

        public static void Update(this MusicPlayComponent self)
        {
            self.CurrentPlayTime += Time.deltaTime;
            switch (self.MusicPlayState)
            {
                case MusicPlayState.Play:
                    self.CurrentAudioTime += Time.deltaTime;
                    break;
                
            }
        }

        #endregion
        
        
        public static void Destroy(this MusicPlayComponent self)
        {
            self.CurrentPlayTime = 0;
            self.CurrentAudioTime = 0;
            GameObject.Destroy(self.MusicPlay);
        }
    }

}