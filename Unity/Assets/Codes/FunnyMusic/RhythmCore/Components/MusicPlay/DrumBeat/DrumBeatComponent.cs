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
        public string AssetName;
        
        /// <summary>
        /// 先用插值移动的方式测试效果
        /// </summary>
        public Transform BeatStart,BeatEnd;

        public float BeatMoveSpeed;
        public Vector3 BeatMoveDirection;
        public float BeatPostActiveTime;
        public float BeatDespawnOffset;
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
            self.Destroy();
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

            self.AssetName = string.Format(ResourcesPath.InternalDrumBeatPath, self.BeatConfig.Prefab);
            self.DrumBeatObject = await RhythmCoreUtil.SpawnDrumBeat(self.AssetName, self.DrumBeatData.ID);
            self.DrumBeatObject.transform.position =
                self.GetParent<TrackControlComponent>().DecisionAppearPoint.transform.position;

            self.BeatStart = self.GetParent<TrackControlComponent>().DecisionAppearPoint;
            self.BeatEnd = self.GetParent<TrackControlComponent>().DecisionTipPoint;
            self.BeatMoveDirection = self.GetParent<TrackControlComponent>().GetBeatDirection();
            self.ActiveState = ActiveState.Active;

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
            float currentMusicTime = musicPlayComponent.CurrentAudioTime;
            self.BeatMoveSpeed = musicPlayComponent.MusicPlaySetting.BeatMoveSpeed;
            self.BeatDespawnOffset = musicPlayComponent.MusicPlaySetting.SpawnTimeRange.y;
            
            self.HybridUpdate(currentMusicTime - beatTime,currentMusicTime,musicPlayComponent.CurrentPlayTime);
            self.UpdateState(currentMusicTime);
            

        }

        private static void UpdateState(this DrumBeatComponent self,float currentTime)
        {
            switch (self.ActiveState)
            {
                case ActiveState.PostActive:
                    //达到了终点，开始计算回收时间
                    if (currentTime - self.BeatPostActiveTime > self.BeatDespawnOffset)
                    {
                        self.Dispose();
                    }
                    
                    break;
                
            }
            
        }

        private static void HybridUpdate(this DrumBeatComponent self,double timeFromStart,float currentMusicTime,float currentPlayTime)
        {
            float perfectTime = 0;
            var deltaT = (float)(timeFromStart - perfectTime);
            
            var direction = self.BeatMoveDirection;
            var distance = deltaT * self.BeatMoveSpeed;
            var targetPosition =self.BeatEnd.transform.position;
            
            var newPosition = targetPosition + (direction * distance);
            self.DrumBeatObject.transform.position = newPosition;

            //到达了目标
            if (deltaT > 0 && self.ActiveState == ActiveState.Active)
            {
                self.ActiveState = ActiveState.PostActive;
                self.BeatPostActiveTime = currentMusicTime;
                FDebug.Print($"完美触发音频时间 {self.BeatPostActiveTime} 关卡时间 {currentPlayTime}");
            }
        }

        public static void Destroy(this DrumBeatComponent self)
        {
            RhythmCoreUtil.DespawnDrumBeat(self.AssetName,self.DrumBeatObject);
        }
        
    }
    
}