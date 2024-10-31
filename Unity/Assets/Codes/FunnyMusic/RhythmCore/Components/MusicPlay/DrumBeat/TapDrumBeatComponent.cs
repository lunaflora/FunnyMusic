using Cysharp.Threading.Tasks;
using FLib;
using Framework;
using RhythmEditor;
using UnityEditor.UI;
using UnityEngine;

namespace FunnyMusic
{
    
    /// <summary>
    /// 点击类鼓点组件
    /// </summary>
    [ComponentOf(typeof(DrumBeat))]
    public class TapDrumBeatComponent : Entity,IAwake<DrumBeatData>,IUpdate,IDestroy
    {

        public DrumBeatData DrumBeatData;

    }
    
    [ObjectSystem]
    public class TapDrumBeatComponentAwakeSystem : AwakeSystem<TapDrumBeatComponent,DrumBeatData>
    {
        public override void Awake(TapDrumBeatComponent self, DrumBeatData drumBeatData)
        {
            self.DrumBeatData = drumBeatData;

        }
    }


    public static class TapDrumBeatComponentSystem
    {

        public static void TriggerDrumBeat(this TapDrumBeatComponent self)
        {
            if (self.GetParent<DrumBeat>().ActiveState == ActiveState.Active)
            {
                //命中
                self.GetParent<DrumBeat>().Hit();
            }
            
        }
        
    }
    
    
}