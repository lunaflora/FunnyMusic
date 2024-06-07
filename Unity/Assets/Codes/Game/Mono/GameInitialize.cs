using System;
using System.Collections;
using System.Collections.Generic;
using FLib;
using UnityEngine;

namespace Game
{
    public class GameInitialize : MonoBehaviour
    {
        [Comment("FrameRate make sure the framerate is high enough on mobile")]
        public int forcedFrameRate = 60;
    
        private void Awake()
        {
            Application.targetFrameRate = forcedFrameRate;
       
        }

        void Start()
        {
       
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }

    
}
