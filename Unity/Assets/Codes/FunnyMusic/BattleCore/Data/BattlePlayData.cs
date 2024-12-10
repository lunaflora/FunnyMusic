using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace FunnyMusic
{
    public enum PlayerAction
    {
        Idle,
        Attack,
        Skill
    }
    
    public partial class UnitInfo
    {
        public long UnitId { get; set; }
        
        public int ConfigId { get; set; }
        
        public int Type { get; set; }

        public float3 Position { get; set; }
      
        public float3 Forward { get; set; }
        
        public Dictionary<int, long> KV { get; set; }
        

    }
}

