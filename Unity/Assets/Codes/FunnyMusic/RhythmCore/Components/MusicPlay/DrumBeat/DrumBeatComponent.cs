using Framework;
using RhythmEditor;

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

    public class DrumBeatComponent : Entity,IAwake<DrumBeatSceneData>,IUpdate,IDestroy
    {
        public DrumBeatSceneData DrumBeatSceneData;
    }
    
    
}