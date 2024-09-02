using UnityEngine;
using UnityEngine.InputSystem;

namespace RhythmEditor
{
    public class RecordModeInput : MonoBehaviour
    {
        public void CrateDrumBeat1(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    EditorEventDefine.EventCrateDrumBeat.SendEventMessage(0);
                    break;
            }
        }
        
        
        public void CrateDrumBeat2(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    EditorEventDefine.EventCrateDrumBeat.SendEventMessage(1);
                    break;
            }
        }
    }
}