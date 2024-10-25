using Cysharp.Threading.Tasks;
using FLib;
using Framework;
using RhythmEditor;
using UnityEngine;

namespace FunnyMusic
{
    /// <summary>
    /// The states for the DrumBeat.
    /// </summary>
    public enum ActiveState
    {
        Disabled,	// When the DrumBeat has not been Initialized yet
        PreActive,	// Between the DrumBeat being intialized and the active state
        Active,		// While the DrumBeat is active
        PostActive	// While the DrumBeat has been deactivated but not reinitialized.
    }

    public class DrumBeatComponent : Entity,IAwake<DrumBeatData>,IUpdate,IDestroy
    {
        
        public DrumBeatSceneData DrumBeatSceneData;
        public DrumBeatData DrumBeatData;
        public ActiveState ActiveState = ActiveState.Disabled;

        public BeatConfig BeatConfig;
        public GameObject DrumBeatObject;
        
        /// <summary>
        /// 先用插值移动的方式测试效果
        /// </summary>
        public Transform BeatStart,BeatEnd;

        public float BeatMoveSpeed;
        public float BeatMoveDirection;
    }
    
    
    [ObjectSystem]
    public class DrumBeatComponentAwakeSystem : AwakeSystem<DrumBeatComponent,DrumBeatData>
    {
        public override void Awake(DrumBeatComponent self, DrumBeatData drumBeatData)
        {
            self.DrumBeatData = drumBeatData;
            self.Initialize().Forget();

        }
    }

    [ObjectSystem]
    public class DrumBeatComponentDestroySystem : DestroySystem<DrumBeatComponent>
    {
        public override void Destroy(DrumBeatComponent self)
        {
            
        }
    }
    
    [ObjectSystem]
    public class DrumBeatComponentUpdateSystem : UpdateSystem<DrumBeatComponent>
    {
        public override void Update(DrumBeatComponent self)
        {
            self.UpdateDrumBeat();
        }
    }


    public static class DrumBeatComponentSystem
    {
        public static async UniTask Initialize(this DrumBeatComponent self)
        {
            int trackID = (int)self.GetParent<TrackControlComponent>().TrackType;
            self.BeatConfig = RhythmCoreUtil.GetBeatConfigByTypeTrack(0, trackID);
            if (self.BeatConfig == null)
            {
                FDebug.Error($"鼓点配置为空 trackID : {trackID}");
            }

            self.DrumBeatObject = await RhythmCoreUtil.SpawnDrumBeat(self.BeatConfig.Prefab, self.DrumBeatData.ID);
            self.DrumBeatObject.transform.position =
                self.GetParent<TrackControlComponent>().DecisionAppearPoint.transform.position;

            self.BeatStart = self.GetParent<TrackControlComponent>().DecisionAppearPoint;
            self.BeatEnd = self.GetParent<TrackControlComponent>().DecisionTipPoint;

        }

        public static void UpdateDrumBeat(this DrumBeatComponent self)
        {
            if (self.DrumBeatObject == null)
            {
                return;
            }
            float beatTime = self.DrumBeatData.BeatTime;
            MusicPlayComponent musicPlayComponent =
                self.GetParent<TrackControlComponent>().GetParent<MusicPlayComponent>();
            float currentTime = musicPlayComponent.CurrentPlayTime;
            self.BeatMoveSpeed = musicPlayComponent.MusicPlaySetting.BeatMoveSpeed;
            self.HybridUpdate(currentTime - beatTime);

        }

        private static void HybridUpdate(this DrumBeatComponent self,double timeFromStart)
        {
            float perfectTime = 0;
            var deltaT = (float)(timeFromStart - perfectTime);
            
        }
        
    }
    
}