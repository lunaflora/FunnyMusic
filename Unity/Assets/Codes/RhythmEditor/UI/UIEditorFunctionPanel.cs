using UnityEngine;

namespace RhythmEditor
{
    /// <summary>
    /// 制谱器工具栏
    /// </summary>
    public class UIEditorFunctionPanel : MonoBehaviour
    {
        #region ButtonFunction

        public void UploadMusic()
        {
            EditorEventDefine.EventUploadMusic.SendEventMessage();
        }
        
        public void Exit()
        {
            #if UNITY_EDITOR

            UnityEditor.EditorApplication.isPlaying = false;

#else

            Application.Quit();

#endif
        }
        

        #endregion
    }
}