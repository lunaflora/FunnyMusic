using System;
using UniFramework.Event;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RhythmEditor
{
    public class InputSystem : MonoBehaviour
    {
        public PlayerInput EditorInput;
        private readonly EventGroup eventGroup = new EventGroup();
        public static InputSystem Input = null;



        public void Awake()
        {
            Input = this;
        }

        private void OnEnable()
        {
           
            eventGroup.AddListener<EditorEventDefine.EventSetCurrentTime>(EventSetCurrentTime);
            eventGroup.AddListener<EditorEventDefine.EventEnterInputMode>(OnEnterInputMode);
            eventGroup.AddListener<EditorEventDefine.EventExitInputMode>(OnExitInputMode);
        }

        private void OnDisable()
        {
            eventGroup.RemoveAllListener();
        }

        public void ToggleInput(bool enable)
        {
            EditorInput.enabled = enable;
        }

        /// <summary>
        /// 手动设置音轨时间，自动退出试玩和录制模式进入编辑模式
        /// </summary>
        /// <param name="message"></param>
        private void EventSetCurrentTime(IEventMessage message)
        {
            switch (EditorDataManager.Instance.SystemMode)
            {
                case SystemMode.DemoMode:
                    EditorInput.SwitchCurrentActionMap("EditorMode");
                    EditorEventSystem.Instance.OnExitDemoMode();
                    break;
                case SystemMode.RecordMode:
                    EditorInput.SwitchCurrentActionMap("EditorMode");
                    EditorEventSystem.Instance.OnExitRecordMode();
                    break;
                
            }

            EditorDataManager.Instance.SystemMode = SystemMode.EditorMode;
        }

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

        public void OnEnterInputMode(IEventMessage message)
        {
            switch (EditorDataManager.Instance.SystemMode)
            {
                case SystemMode.EditorMode:
                    EditorInput.SwitchCurrentActionMap("InputMode");
                    break;
                default:
                    return;
                
            }

            EditorDataManager.Instance.SystemMode = SystemMode.InputMode;

        }
       
        public void OnExitInputMode(IEventMessage message)
        {
            switch (EditorDataManager.Instance.SystemMode)
            {
                case SystemMode.InputMode:
                    EditorInput.SwitchCurrentActionMap("EditorMode");
                    break;
                default:
                    return;
                
            }

            EditorDataManager.Instance.SystemMode = SystemMode.EditorMode;
        }
    }
}