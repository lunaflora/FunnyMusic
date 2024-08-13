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


        private void OnExitDemo()
        {
            BGMSource.Pause();
        }
        private void OnEnterDemo()
        {
            EditorDataManager.Instance.CurrentTime = BGMSource.time;
            BGMSource.Play();
        }

        private void DoRecord()
        {
            EditorDataManager.Instance.CurrentTime = BGMSource.time;
        }



        #region LifeCircle

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            switch (EditorDataManager.Instance.SystemMode)
            {
                case SystemMode.DemoMode:
                    OnEnterDemo();
                    break;
                case SystemMode.RecordMode:
                    DoRecord();
                    break;
                
            }
        }

        private void OnEnable()
        {
            EditorEventSystem.Instance.OnEnterDemoMode += OnEnterDemo;
            EditorEventSystem.Instance.OnExitDemoMode += OnExitDemo;
            EditorEventSystem.Instance.OnEnterRecordMode += OnEnterDemo;
            EditorEventSystem.Instance.OnExitRecordMode += OnExitDemo;
        }

        private void OnDisable()
        {
            EditorEventSystem.Instance.OnEnterDemoMode -= OnEnterDemo;
            EditorEventSystem.Instance.OnExitDemoMode -= OnExitDemo;
            EditorEventSystem.Instance.OnEnterRecordMode -= OnEnterDemo;
            EditorEventSystem.Instance.OnExitRecordMode -= OnExitDemo;
        }

        #endregion
    }
}