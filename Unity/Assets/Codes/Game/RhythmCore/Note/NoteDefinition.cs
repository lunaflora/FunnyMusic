using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    [CreateAssetMenu(fileName = "My Note Definition", menuName = "RhythmCore/Note Definition", order = 1)]
    public class NoteDefinition : ScriptableObject
    {
        public enum ClipDurationType
        {
            Free,
            Crochet,  //
            HalfCrochet,
            QuarterCrochet,
            ScaledCrochet,
        }
    }
}