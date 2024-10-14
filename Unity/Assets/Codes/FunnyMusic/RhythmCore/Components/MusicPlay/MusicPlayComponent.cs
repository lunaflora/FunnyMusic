using Framework;
using UnityEngine;

namespace FunnyMusic
{
    /// <summary>
    /// 节奏音乐核心逻辑组件
    /// </summary>
    [ChildType(typeof(TrackControlComponent))]
    public class MusicPlayComponent : Entity,IAwake<GameObject>,IDestroy
    {
        
    }
}