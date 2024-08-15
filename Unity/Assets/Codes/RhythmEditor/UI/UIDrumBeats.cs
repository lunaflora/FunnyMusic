using System;
using UnityEngine;

namespace RhythmEditor
{
    /// <summary>
    /// 鼓点界面
    /// </summary>
    public class UIDrumBeats : MonoBehaviour
    {
        public RectTransform[] WaveformUITracks;

        
        private void Initialize()
        {
            float audioLength = EditorDataManager.Instance.LoadingAudio.length;
            for (int i = 0; i < WaveformUITracks.Length; i++)
            {
                WaveformUITracks[i].sizeDelta = new Vector2(audioLength * UIConstValue.UIWidthScale, 50f);
            }
        }
        
        private void Start()
        {
            Initialize();
        }
    }
}