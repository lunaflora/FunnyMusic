using System.Collections.Generic;
using UnityEngine;

namespace FunnyMusic
{
    
    public class AnimationMap : MonoBehaviour
    {
        [SerializeField] 
        public Dictionary<PlayerAction, AnimationClip> PlayerAnimationMap;

    }
}
