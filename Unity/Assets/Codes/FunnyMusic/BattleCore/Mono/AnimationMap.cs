using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace FunnyMusic
{
    
    public class AnimationMap : SerializedMonoBehaviour
    {
     
        [NonSerialized, OdinSerialize]
        public Dictionary<PlayerAction, AnimationClip> PlayerAnimationMap;


    }
}

