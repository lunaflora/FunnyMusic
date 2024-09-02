using System;
using UniFramework.Event;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RhythmEditor
{
    /// <summary>
    ///游戏音乐管理器
    /// </summary>
    public class EditMusicManager : MonoBehaviour
    {
        public AudioSource BGMSource;
        private readonly EventGroup eventGroup = new EventGroup();
        
        private void Initialize()
        {
            if (EditorDataManager.Instance.LoadingAudio != null)
            {
                BGMSource.clip = EditorDataManager.Instance.LoadingAudio;
            }
            
        }


        /// <summary>
        /// 离开试玩模式
        /// </summary>
        private void OnExitDemo()
        {
            BGMSource.Pause();
        }
        
        /// <summary>
        /// 进入试玩模式
        /// </summary>
        private void OnEnterDemo()
        {
            EditorDataManager.Instance.CurrentTime = BGMSource.time;
            BGMSource.Play();
        }

        /// <summary>
        /// 录制模式Loop
        /// </summary>
        private void DoRecord()
        {
            EditorDataManager.Instance.CurrentTime = BGMSource.time;
            EditorEventDefine.EventUpdateCurrentTime.SendEventMessage();
            
            if (BGMSource.clip.length - BGMSource.time < 0.02f)
            {
                EditorEventDefine.EventSetCurrentTime.SendEventMessage(0);
            }
        }

        /// <summary>
        /// 试玩模式Loop
        /// </summary>
        private void DoDemo()
        {
            EditorDataManager.Instance.CurrentTime = BGMSource.time;
            EditorEventDefine.EventUpdateCurrentTime.SendEventMessage();

            if (BGMSource.clip.length - BGMSource.time < 0.02f)
            {
                EditorEventDefine.EventSetCurrentTime.SendEventMessage(0);
            }
        }

        private void EventSetCurrentTime(IEventMessage message)
        {
            EditorEventDefine.EventSetCurrentTime setCurrentTime = message as EditorEventDefine.EventSetCurrentTime;
            BGMSource.time = setCurrentTime.CurrentTime;
          
        }

        private void UploadMusicComplete(IEventMessage message)
        {
            Initialize();
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
                    DoDemo();
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
            eventGroup.AddListener<EditorEventDefine.EventUploadMusicComplete>(UploadMusicComplete);
            eventGroup.AddListener<EditorEventDefine.EventSetCurrentTime>(EventSetCurrentTime);
        }

        private void OnDisable()
        {
            EditorEventSystem.Instance.OnEnterDemoMode -= OnEnterDemo;
            EditorEventSystem.Instance.OnExitDemoMode -= OnExitDemo;
            EditorEventSystem.Instance.OnEnterRecordMode -= OnEnterDemo;
            EditorEventSystem.Instance.OnExitRecordMode -= OnExitDemo;
            
            eventGroup.RemoveAllListener();
        }

        #endregion

        
    }
}