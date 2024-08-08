using Core;
using DigitalRubyShared;
using FLib;
using ParadoxNotion;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Game
{
    public class InputSystemHandler : Singleton<InputSystemHandler>
    {
        #region GestureRecognizer

        /// <summary>
        /// 单击
        /// </summary>
        private TapGestureRecognizer tapGesture;
        /// <summary>
        /// 滑动
        /// </summary>
        private SwipeGestureRecognizer swipeGesture;

        #endregion

        public override void Initialize()
        {
            TouchSimulation.Enable();
            InitGestureRecognizer();
        }

        private void InitGestureRecognizer()
        {
            CreateTapGesture();
            CreateSwipeGesture();
        }
        
        #region InputEvent

        private void CreateTapGesture()
        {
            tapGesture = new TapGestureRecognizer();
            tapGesture.StateUpdated += TapGestureCallback;
            FingersScript.Instance.AddGesture(tapGesture);
        }

        private void TapGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
        {
          
            if (gesture.State == GestureRecognizerState.Ended)
            {
                FDebug.Print($"Single tapped at {gesture.FocusX}, {gesture.FocusY}");
                HandleTouchNote(gesture.FocusX, gesture.FocusY, 0.2f);

             
            }

        }

        
        private void CreateSwipeGesture()
        {
            swipeGesture = new SwipeGestureRecognizer();
            swipeGesture.Direction = SwipeGestureRecognizerDirection.Any;
            swipeGesture.StateUpdated += SwipeGestureCallback;
            swipeGesture.DirectionThreshold = 1.0f; // allow a swipe, regardless of slope
            FingersScript.Instance.AddGesture(swipeGesture);
        }

        private void SwipeGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                HandleSwipeNote(gesture.FocusX, gesture.FocusY);
             
            }
        }
        

        #endregion
       
        private void HandleTouchNote(float screenX, float screenY, float radius)
        {
            
        }

        private void HandleSwipeNote(float endX, float endY)
        {
           
            
            
          
        }
        
    }
}