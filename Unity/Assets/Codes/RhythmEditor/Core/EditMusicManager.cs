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
            EditorDataManager.Instance.CurrentPlayTime = 0;
            BGMSource.Pause();
        }
        
        /// <summary>
        /// 进入试玩模式
        /// </summary>
        private void OnEnterDemo()
        {
            EditorDataManager.Instance.CurrentPlayTime = BGMSource.time;
            BGMSource.Play();
        }

        /// <summary>
        /// 录制模式Loop
        /// </summary>
        private void DoRecord()
        {
            EditorDataManager.Instance.CurrentAudioTime = BGMSource.time;
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
            EditorDataManager.Instance.CurrentAudioTime = BGMSource.time;
            EditorDataManager.Instance.CurrentPlayTime += Time.deltaTime;
            EditorEventDefine.EventUpdateCurrentTime.SendEventMessage();
            

            //Debug.Log($"{Time.deltaTime}    {EditorDataManager.Instance.CurrentAudioTime}");

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
            eventGroup.AddListener<EditorEventDefine.EventSetMainVolume>(OnSetMainVolume);
            eventGroup.AddListener<EditorEventDefine.EventSetSongVolume>(OnSetSongVolume);
            eventGroup.AddListener<EditorEventDefine.EventSetBackgoundVolume>(OnSetBackgoundVolume);
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


        #region SetVolume

        private void OnSetBackgoundVolume(IEventMessage eventMessage)
        {
            EditorEventDefine.EventSetBackgoundVolume eventSetBackgoundVolume =
                eventMessage as EditorEventDefine.EventSetBackgoundVolume;

            BGMSource.outputAudioMixerGroup.audioMixer.SetFloat("BackGroundVolume", eventSetBackgoundVolume.Volume);

        }

        private void OnSetSongVolume(IEventMessage eventMessage)
        {
            EditorEventDefine.EventSetSongVolume eventSetSongVolume =
                eventMessage as EditorEventDefine.EventSetSongVolume;
            
            BGMSource.outputAudioMixerGroup.audioMixer.SetFloat("SongVolume", eventSetSongVolume.Volume);
        }

        private void OnSetMainVolume(IEventMessage eventMessage)
        {
            EditorEventDefine.EventSetMainVolume eventSetMainVolume =
                eventMessage as EditorEventDefine.EventSetMainVolume;
            
            BGMSource.outputAudioMixerGroup.audioMixer.SetFloat("MainVolume", eventSetMainVolume.Volume);
        }

        

        #endregion

        
    }
}