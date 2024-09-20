using System;
using TMPro;
using UniFramework.Event;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RhythmEditor
{
    /// <summary>
    /// 音频播放进度指针
    /// </summary>
    public class TimeFrame : MonoBehaviour, IPointerClickHandler
    {
        private readonly EventGroup eventGroup = new EventGroup();

        #region UI Element

        public RectTransform UITimeFrame;
        public TextMeshProUGUI TextTimeFrame;
        

        #endregion

        private void OnEnable()
        {
            eventGroup.AddListener<EditorEventDefine.EventUpdateCurrentTime>(EventUpdateCurrentTime);
            eventGroup.AddListener<EditorEventDefine.EventSetCurrentTime>(EventSetCurrentTime);
        }

        private void OnDisable()
        {
            eventGroup.RemoveAllListener();
        }

        private void EventUpdateCurrentTime(IEventMessage message)
        {
            float newTime = EditorDataManager.Instance.CurrentAudioTime;
            UITimeFrame.anchoredPosition = new Vector2(newTime * UIConstValue.UIWidthScale, 0);
            TextTimeFrame.text = newTime.ToString(".00");
        }

        private void EventSetCurrentTime(IEventMessage message)
        {
            EditorEventDefine.EventSetCurrentTime setCurrentTime = message as EditorEventDefine.EventSetCurrentTime;
            UITimeFrame.anchoredPosition = new Vector2(setCurrentTime.CurrentTime * UIConstValue.UIWidthScale, 0);
            TextTimeFrame.text = setCurrentTime.CurrentTime.ToString(".00");
            EditorDataManager.Instance.CurrentAudioTime = setCurrentTime.CurrentTime;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            UITimeFrame.position = eventData.pressPosition;
            EditorEventDefine.EventSetCurrentTime.SendEventMessage(UITimeFrame.anchoredPosition.x / UIConstValue.UIWidthScale);
        }
    }
}