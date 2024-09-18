using UnityEngine;
using UnityEngine.InputSystem;

namespace RhythmEditor
{
    public class DemoModelInput : MonoBehaviour
    {

        /// <summary>
        /// Demo模式打击类型1
        /// </summary>
        /// <param name="context"></param>
        public void DemoPoint1(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    EditorEventDefine.EventDemoPoint1.SendEventMessage();
                    break;
                
            }
        }
        
        /// <summary>
        /// Demo模式打击类型2
        /// </summary>
        /// <param name="context"></param>
        public void DemoPoint2(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    EditorEventDefine.EventDemoPoint2.SendEventMessage();
                    break;
                
            }
        }
        
    }
}