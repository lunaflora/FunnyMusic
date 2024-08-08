using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace RhythmEditor
{
    public class EditorDataManager : MonoBehaviourSingleton<EditorDataManager>
    {
        [SerializeField]
        public SystemMode SystemMode = SystemMode.EditorMode;

    }
    
}

