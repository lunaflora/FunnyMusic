using System;
using System.Collections;
using System.Collections.Generic;
using FLib;
using RhythmTool;
using UnityEngine;

namespace Test
{
    public class AnalyzeExample : MonoBehaviour
    {
        public RhythmData rhythmData;
        public AudioSource audioSource;


        private void Awake()
        {
            foreach (var track in rhythmData.tracks)
            {
                FDebug.Print(track.name);
            }
        }
    }

}


