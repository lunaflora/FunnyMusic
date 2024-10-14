using Framework;
using UnityEngine;

namespace FunnyMusic
{
    /// <summary>
    /// 战斗核心逻辑组件
    /// </summary>
    public class BattleCoreComponent : Entity,IAwake<GameObject>,IDestroy
    {
        #region Music Spped

        public float Bpm = 120;
        
        public float Crochet => 60f / Bpm;
        public float HalfCrochet => 30f / Bpm;
        public float QuarterCrochet => 15f / Bpm;
        

        #endregion
     
        
    }
}