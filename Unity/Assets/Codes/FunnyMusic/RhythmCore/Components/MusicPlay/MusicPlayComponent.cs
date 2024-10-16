using Codes.FunnyMusic.RhythmCore.SO;
using Core;
using Cysharp.Threading.Tasks;
using FLib;
using Framework;
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
            //1.加载制谱表
            
            //2.加载MusicPlayData
            self.MusicPlaySetting =
                await AssetLoaderSystem.Instance.LoadAssetAsync<MusicPlaySetting>(
                    $"{ResourcesPath.InternalLevelConfig}{levelName}_play.asset");
            
            FDebug.Print($"MusicPlaySetting {self.MusicPlaySetting.BeatSpeed}");


        }
    }

}