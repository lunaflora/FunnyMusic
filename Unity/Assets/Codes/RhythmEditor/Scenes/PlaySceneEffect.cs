using System;
using UniFramework.Event;
using UnityEngine;
using UnityEngine.VFX;

namespace RhythmEditor
{
    public class PlaySceneEffect : MonoBehaviour
    {
        private readonly EventGroup eventGroup = new EventGroup();
        public VisualEffect PlayEffect;
        
        
        private void OnEnable()
        {
            eventGroup.AddListener<EditorEventDefine.EventDemoDrumMiss>(OnDemoDrumMiss);
            eventGroup.AddListener<EditorEventDefine.EventDemoDrumCool>(OnDemoDrumCool);
            eventGroup.AddListener<EditorEventDefine.EventDemoDrumGreat>(OnDemoDrumGreat);
            eventGroup.AddListener<EditorEventDefine.EventDemoDrumBad>(OnDemoDrumBad);
            
            eventGroup.AddListener<EditorEventDefine.EventDemoPoint1>(OnDemoPoint1);
            eventGroup.AddListener<EditorEventDefine.EventDemoPoint2>(OnDemoPoint2);
        }

        private void OnDisable()
        {
            eventGroup.RemoveAllListener();
        }

        private void Update()
        {
           
        }
        

        private void OnDemoPoint2(IEventMessage eventMessage)
        {
            PlayEffect.SendEvent("Beat");
        }

        private void OnDemoPoint1(IEventMessage eventMessage)
        {
            PlayEffect.SendEvent("Beat");
        }

        private void OnDemoDrumBad(IEventMessage eventMessage)
        {
            PlayEffect.SendEvent("Bad");
        }

        private void OnDemoDrumGreat(IEventMessage eventMessage)
        {
            PlayEffect.SendEvent("Great");
        }

        private void OnDemoDrumCool(IEventMessage eventMessage)
        {
            PlayEffect.SendEvent("Cool");
        }

        private void OnDemoDrumMiss(IEventMessage eventMessage)
        {
            PlayEffect.SendEvent("Miss");
        }

        private void OnDestroy()
        {
            
        }
    }
}