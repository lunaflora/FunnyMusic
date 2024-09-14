using UnityEngine;

namespace RhythmEditor
{
    /// <summary>
    /// 输入模式 防止在键盘输入的同时触发其他编辑器按键事件
    /// </summary>
    public class UIInputMode : MonoBehaviour
    {

        public void EnterInputMode()
        {
            EditorEventDefine.EventEnterInputMode.SendEventMessage();
        }

        public void ExitInputMode()
        {
            EditorEventDefine.EventExitInputMode.SendEventMessage();
        }
        
    }
}