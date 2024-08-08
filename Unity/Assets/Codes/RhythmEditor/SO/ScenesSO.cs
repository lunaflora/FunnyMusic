using System.Collections.Generic;
using UnityEngine;

namespace RhythmEditor
{
    [CreateAssetMenu(fileName = "ScenesSO", menuName = "RhythmEditor/ScenesSO", order = 0)]
    public class ScenesSO : ScriptableObject
    {
        [SerializeField]
        public int SceneGroupID;

        [SerializeField] 
        public List<string> SceneNameList = new List<string>();
    }
}