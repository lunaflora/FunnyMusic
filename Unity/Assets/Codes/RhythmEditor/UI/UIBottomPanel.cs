using System;
using DG.Tweening;
using UniFramework.Event;
using UnityEngine;
using UnityEngine.UI;

namespace RhythmEditor
{
    /// <summary>
    /// 底部面板
    /// </summary>
    public class UIBottomPanel : MonoBehaviour
    {
        public RectTransform BottomPanel;
        public Image ButtonImage;
        public Sprite[] SwitchSprites;
        public RectTransform ScrollAreaContent;

        private readonly EventGroup eventGroup = new EventGroup();

        private void Initialize()
        {
            float audioLength = EditorDataManager.Instance.LoadingAudio.length;
            ScrollAreaContent.sizeDelta = new Vector2(audioLength * UIConstValue.UIWidthScale, 300);
        }
        
        /// <summary>
        /// 开关底部面板
        /// </summary>
        public void ToggleBottomPanel()
        {
            if (BottomPanel.anchoredPosition == new Vector2(0, 50))
            {
                BottomPanel.anchoredPosition = new Vector2(0, 300);
                ButtonImage.sprite = SwitchSprites[0];
            }
            else
            {
                BottomPanel.anchoredPosition = new Vector2(0, 50);
                ButtonImage.sprite = SwitchSprites[1];
            }
        }

        private void UploadMusicComplete(IEventMessage eventMessage)
        {
            Initialize();
        }

        #region Life

        private void Start()
        {
            Initialize();
          
        }

        private void OnEnable()
        {
            eventGroup.AddListener<EditorEventDefine.EventUploadMusicComplete>(UploadMusicComplete);
        }

        private void OnDisable()
        {
            eventGroup.RemoveAllListener();
        }

        #endregion
        

    }
}