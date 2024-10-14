using Core;
using DigitalRubyShared;
using FLib;
using Framework;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace FunnyMusic
{

    /// <summary>
    /// 这个是管理玩家各种输入输出操作的Component
    /// </summary>
    [ComponentOf(typeof(World))]
    public class InputComponent : Entity, IAwake, IUpdate
    {
        #region GestureRecognizer

        /// <summary>
        /// 单击
        /// </summary>
        public TapGestureRecognizer TapGesture;
        /// <summary>
        /// 滑动
        /// </summary>
        public SwipeGestureRecognizer SwipeGesture;

        #endregion

    }
    
    [ObjectSystem]
    public class InputComponentAwakeSystem : AwakeSystem<InputComponent>
    {
        public override void Awake(InputComponent self)
        {
            self.Initialize();
        }
    }

    public static class InputComponentSystem
    {

        public static void Initialize(this InputComponent self)
        {
            TouchSimulation.Enable();
            self.InitGestureRecognizer();
            
        }

        public static void InitGestureRecognizer(this InputComponent self)
        {
            self.CreateTapGesture();
            self.CreateSwipeGesture();
        }
        
        #region InputEvent

        private static void CreateTapGesture(this InputComponent self)
        {
            self.TapGesture = new TapGestureRecognizer();
            self.TapGesture.StateUpdated += self.TapGestureCallback;
            FingersScript.Instance.AddGesture(self.TapGesture);
        }

        private static void TapGestureCallback(this InputComponent self,DigitalRubyShared.GestureRecognizer gesture)
        {
          
            if (gesture.State == GestureRecognizerState.Ended)
            {
                FDebug.Print($"Single tapped at {gesture.FocusX}, {gesture.FocusY}");
                
            }

        }
        

        
        private static void CreateSwipeGesture(this InputComponent self)
        {
            self.SwipeGesture = new SwipeGestureRecognizer();
            self.SwipeGesture .Direction = SwipeGestureRecognizerDirection.Any;
            self.SwipeGesture .StateUpdated += self.SwipeGestureCallback;
            self.SwipeGesture .DirectionThreshold = 1.0f; // allow a swipe, regardless of slope
            FingersScript.Instance.AddGesture(self.SwipeGesture );
        }

        private static void SwipeGestureCallback(this InputComponent self,DigitalRubyShared.GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                
             
            }
        }
        

        #endregion

    }

    
}