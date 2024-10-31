using FLib;
using UniFramework.Event;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FunnyMusic
{
    public class GameInputSetting : MonoBehaviour
    {
        public PlayerInput EditorInput;
        private readonly EventGroup eventGroup = new EventGroup();
        
        
        /// <summary>
        /// 触发第一轨道
        /// </summary>
        /// <param name="context"></param>
        public void OnTriggerTack0(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    FDebug.Print("触发第一轨道");
                    InputEventData inputEventData = new InputEventData((int)TrackType.LeftTrack,0);
                    EventDefine.EventTriggerTrack.SendEventMessage(inputEventData);
                    break;
                
            }
        }
        
        /// <summary>
        /// 触发第二轨道
        /// </summary>
        /// <param name="context"></param>
        public void OnTriggerTack1(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    FDebug.Print("触发第二轨道");
                    InputEventData inputEventData = new InputEventData((int)TrackType.LeftTrack,1);
                    EventDefine.EventTriggerTrack.SendEventMessage(inputEventData);
                    break;
                
            }
        }
        
    }
    
}