using UnityEngine;
using UnityEngine.InputSystem;

namespace RhythmEditor
{
    public class EditorInputSystem : MonoBehaviour
    {
        public PlayerInput EditorInput;

        public void SwitchRecordMode(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    switch (EditorDataManager.Instance.SystemMode)
                    {
                        case SystemMode.RecordMode:
                            EditorInput.SwitchCurrentActionMap("EditorMode");
                            EditorDataManager.Instance.SystemMode = SystemMode.EditorMode;
                            break;
                        case SystemMode.EditorMode:
                            EditorInput.SwitchCurrentActionMap("RecordMode");
                            EditorDataManager.Instance.SystemMode = SystemMode.RecordMode;
                            break;
                        
                    }
                    break;
                
            }
        }
        public void SwitchDemoMode(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    switch (EditorDataManager.Instance.SystemMode)
                    {
                        case SystemMode.DemoMode:
                            EditorInput.SwitchCurrentActionMap("EditorMode");
                            EditorDataManager.Instance.SystemMode = SystemMode.EditorMode;
                            break;
                        case SystemMode.EditorMode:
                            EditorInput.SwitchCurrentActionMap("DemoMode");
                            EditorDataManager.Instance.SystemMode = SystemMode.DemoMode;
                            break;
                        
                    }
                    break;
                
            }
        }

       
    }
}