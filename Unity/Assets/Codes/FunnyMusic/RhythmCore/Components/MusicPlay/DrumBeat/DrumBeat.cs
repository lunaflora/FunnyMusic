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

    
    public class DrumBeat : Entity,IAwake<DrumBeatData>,IUpdate,IDestroy
    {
        public DrumBeatSceneData DrumBeatSceneData;
        public DrumBeatData DrumBeatData;
        public ActiveState ActiveState = ActiveState.Disabled;

        public BeatConfig BeatConfig;
        public BeatType BeatType => (BeatType)BeatConfig.BeatType;
        public GameObject DrumBeatObject;
        public string AssetName;
        
     
        public Transform BeatStart,BeatEnd;

        public float BeatMoveSpeed;
        public Vector3 BeatMoveDirection;
        public float BeatPostActiveTime;
        public float BeatDisableOffset;
        public float BeatActiveOffset;
        public float BeatDeactivateOffset;

    }
    
    [ObjectSystem]
    public class DrumBeatAwakeSystem : AwakeSystem<DrumBeat,DrumBeatData>
    {
        public override void Awake(DrumBeat self, DrumBeatData drumBeatData)
        {
            self.DrumBeatData = drumBeatData;
            self.Initialize().Forget();

        }
    }

    [ObjectSystem]
    public class DrumBeatDestroySystem : DestroySystem<DrumBeat>
    {
        public override void Destroy(DrumBeat self)
        {
            self.Destroy();
        }
    }
    
    [ObjectSystem]
    public class DrumBeatUpdateSystem : UpdateSystem<DrumBeat>
    {
        public override void Update(DrumBeat self)
        {
            self.UpdateDrumBeat();
        }
    }


    public static class DrumBeatComponentSystem
    {
        public static async UniTask Initialize(this DrumBeat self)
        {
            int trackID = (int)self.GetParent<TrackControlComponent>().TrackType;
            self.BeatConfig = RhythmCoreUtil.GetBeatConfigByTypeTrack(0, trackID);
            if (self.BeatConfig == null)
            {
                FDebug.Error($"鼓点配置为空 trackID : {trackID}");
            }
            self.AddTypeComponent();

            self.AssetName = string.Format(ResourcesPath.InternalDrumBeatPath, self.BeatConfig.Prefab);
            self.DrumBeatObject = await RhythmCoreUtil.SpawnDrumBeat(self.AssetName, self.DrumBeatData.ID);
            self.DrumBeatObject.transform.position =
                self.GetParent<TrackControlComponent>().DecisionAppearPoint.transform.position;

            self.BeatStart = self.GetParent<TrackControlComponent>().DecisionAppearPoint;
            self.BeatEnd = self.GetParent<TrackControlComponent>().DecisionTipPoint;
            self.BeatMoveDirection = self.GetParent<TrackControlComponent>().GetBeatDirection();

            self.BeatActiveOffset = RhythmCoreUtil.ConvertIntToFloat(self.BeatConfig.TriggerRange[0]);
            self.BeatDeactivateOffset = RhythmCoreUtil.ConvertIntToFloat(self.BeatConfig.TriggerRange[1]);
            self.ActiveState = ActiveState.PreActive;

        }

        public static void AddTypeComponent(this DrumBeat self)
        {
            switch (self.BeatType)
            {
                case BeatType.Tap:
                    self.AddComponent<TapDrumBeatComponent, DrumBeatData>(self.DrumBeatData);
                    break;
                
            }
        }

        public static void UpdateDrumBeat(this DrumBeat self)
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
            self.BeatDisableOffset = musicPlayComponent.MusicPlaySetting.SpawnTimeRange.y;

            self.HybridUpdate(currentMusicTime - beatTime, currentMusicTime, musicPlayComponent.CurrentPlayTime);
            self.UpdateState(currentMusicTime - beatTime, currentMusicTime, musicPlayComponent.CurrentPlayTime);


        }

        private static void UpdateState(this DrumBeat self, double timeFromStart, float currentMusicTime,
            float currentPlayTime)
        {
            float perfectTime = 0;
            var deltaT = (float)(timeFromStart - perfectTime);

            switch (self.ActiveState)
            {
                case ActiveState.PreActive:
                    //到达了激活时间
                    if (deltaT > -self.BeatActiveOffset)
                    {
                        self.ActiveState = ActiveState.Active;
                        self.GetParent<TrackControlComponent>().SetActiveBeat(self.Id);
                        FDebug.Print($"鼓点激活 {self.Id}  关卡时间 {currentPlayTime}");
                    }


                    break;

                case ActiveState.PostActive:

                    //达到了终点，开始计算超时
                    if (deltaT > self.BeatDeactivateOffset)
                    {
                        self.GetParent<TrackControlComponent>().RemoveActiveBeat(self.Id);
                        self.ActiveState = ActiveState.Disabled;
                        FDebug.Print($"鼓点超时 {self.Id}  关卡时间 {currentPlayTime}");
                    }

                    break;
                case ActiveState.Disabled:

                    //超过了触发时间，开始计算回收时间
                    if (currentMusicTime - self.BeatPostActiveTime > self.BeatDisableOffset)
                    {
                        FDebug.Print($"鼓点删除 {self.Id}  关卡时间 {currentPlayTime}");
                        self.Dispose();
                    }

                    break;
                case ActiveState.Active:
                    //到达了目标
                    if (deltaT > 0 && self.ActiveState == ActiveState.Active)
                    {
                        self.ActiveState = ActiveState.PostActive;
                        self.BeatPostActiveTime = currentMusicTime;
                        FDebug.Print($"完美触发音频时间 {self.BeatPostActiveTime} 关卡时间 {currentPlayTime}");
                    }

                    break;

            }

        }

        private static void HybridUpdate(this DrumBeat self, double timeFromStart, float currentMusicTime,
            float currentPlayTime)
        {
            float perfectTime = 0;
            var deltaT = (float)(timeFromStart - perfectTime);

            var direction = self.BeatMoveDirection;
            var distance = deltaT * self.BeatMoveSpeed;
            var targetPosition = self.BeatEnd.transform.position;

            var newPosition = targetPosition + (direction * distance);
            self.DrumBeatObject.transform.position = newPosition;


        }

        /// <summary>
        /// 命中鼓点
        /// </summary>
        public static void Hit(this DrumBeat self)
        {
            self.GetParent<TrackControlComponent>().RemoveActiveBeat(self.Id);
            self.Dispose();
        }

        public static void Destroy(this DrumBeat self)
        {
            RhythmCoreUtil.DespawnDrumBeat(self.AssetName, self.DrumBeatObject);
        }
    }
}