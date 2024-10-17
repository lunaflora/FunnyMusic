using System.Collections.Generic;
using Codes.FunnyMusic.RhythmCore.SO;
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
    public class MusicPlayComponent : Entity,IAwake<string>,IDestroy
    {
        /// <summary>
        /// 主控制器
        /// </summary>
        public GameObject MusicPlay;
        /// <summary>
        /// 主音乐
        /// </summary>
        public AudioSource PlayMusicSource;

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
    
    public static class MusicPlayComponentSystem
    {
        public static async UniTask Initialize(this MusicPlayComponent self,string levelName)
        {
            //1.加载MusicPlay预制
            self.MusicPlay = await AssetLoaderSystem.Instance.InstantiateSync(ResourcesPath.InternalMusicPlayPath,
                GlobalGameObjectComponent.Instance.Controller);
            
            //2.加载制谱表
            await self.LoadMusicLevel(levelName);
            
            //3.加载MusicPlayData
            self.MusicPlaySetting =
                await AssetLoaderSystem.Instance.LoadAssetAsync<MusicPlaySetting>(
                    $"{ResourcesPath.InternalLevelConfig}{levelName}_play.asset");
            
            FDebug.Print($"MusicPlaySetting {self.MusicPlaySetting.BeatSpeed}");


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
            }
        }
    }

}