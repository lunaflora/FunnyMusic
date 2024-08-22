using UnityEngine;
using UnityEngine.InputSystem;

namespace RhythmEditor
{
    public class InputSystem : MonoBehaviour
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
                            EditorEventDefine.EventExitRecordMode.SendEventMessage();
                            break;
                        case SystemMode.EditorMode:
                            EditorInput.SwitchCurrentActionMap("RecordMode");
                            EditorDataManager.Instance.SystemMode = SystemMode.RecordMode;
                            EditorEventDefine.EventEnterDemoMode.SendEventMessage();
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
                            EditorEventDefine.EventExitDemoMode.SendEventMessage();
                            break;
                        case SystemMode.EditorMode:
                            EditorInput.SwitchCurrentActionMap("DemoMode");
                            EditorDataManager.Instance.SystemMode = SystemMode.DemoMode;
                            EditorEventDefine.EventEnterDemoMode.SendEventMessage();
                            break;
                        
                    }
                    break;
                
            }
        }

       
    }
}