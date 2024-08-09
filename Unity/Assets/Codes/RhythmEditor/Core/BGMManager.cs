using System;
using UnityEngine;

namespace RhythmEditor
{
    /// <summary>
    /// 背景音乐管理器
    /// ?游戏音乐管理器
    /// </summary>
    public class BGMManager : MonoBehaviour
    {
        public AudioSource BGMSource;
        
        private void Initialize()
        {
            if (EditorDataManager.Instance.LoadingAudio != null)
            {
                BGMSource.clip = EditorDataManager.Instance.LoadingAudio;
            }
            
        }

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            switch (EditorDataManager.Instance.SystemMode)
            {
                case SystemMode.DemoMode:
                    DoDemo();
                    break;
                case SystemMode.RecordMode:
                    DoRecord();
                    break;
                
            }
        }

        private void DoDemo()
        {
            EditorDataManager.Instance.CurrentTime = BGMSource.time;
        }

        private void DoRecord()
        {
            EditorDataManager.Instance.CurrentTime = BGMSource.time;
        }
    }
}