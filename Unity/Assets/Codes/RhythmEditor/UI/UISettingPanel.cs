using System;
using TMPro;
using UniFramework.Event;
using UnityEngine;
using UnityEngine.UI;

namespace RhythmEditor
{
    public class UISettingPanel : MonoBehaviour
    {
        public Slider MainVolumeValue, BackVolumeValue, SongVolumeValue;
        public TMP_InputField CoolValue, GreatValue, BadValue, MissValue;
        public TMP_Dropdown LevelDifficulty;
        
        private readonly EventGroup eventGroup = new EventGroup();


        private void OnEnable()
        {
            eventGroup.AddListener<EditorEventDefine.EventLoadedLevelData>(OnLoadedLevelData);
        }
        private void OnDisable()
        {
            eventGroup.RemoveAllListener();
        }

        private void Start()
        {
            InitDifficultyOption();
            OnLoadedLevelData(null);
        }

        private void InitDifficultyOption()
        {
            LevelDifficulty.ClearOptions();
            string[] options = System.Enum.GetNames(typeof(RhythmEditor.LevelDifficulty));
            foreach (var option in options)
            {
                LevelDifficulty.options.Add(new TMP_Dropdown.OptionData(option));
            }
            
        }


        private void OnLoadedLevelData(IEventMessage obj)
        {
            SetMainVolume(EditorDataManager.Instance.MainVolume);
            SetBackVolume(EditorDataManager.Instance.BackGroundVolume);
            SetSongVolume(EditorDataManager.Instance.SongVolume);
            
            SetCoolValue(EditorDataManager.Instance.CoolValue.ToString());
            SetGreatValue(EditorDataManager.Instance.GreatValue.ToString());
            SetBadValue(EditorDataManager.Instance.BadValue.ToString());
            SetMissValue(EditorDataManager.Instance.MissValue.ToString());
            SetDifficulty((int)EditorDataManager.Instance.LevelDifficulty);
        }


        #region Setting

        public void SetMainVolume(float volume)
        {
            MainVolumeValue.value = volume;
            EditorDataManager.Instance.MainVolume = volume;
            EditorEventDefine.EventSetMainVolume.SendEventMessage(volume);
        }

        public void SetBackVolume(float volume)
        {
            BackVolumeValue.value = volume;
            EditorDataManager.Instance.BadValue = volume;
            EditorEventDefine.EventSetBackgoundVolume.SendEventMessage(volume);
        }

        public void SetSongVolume(float volume)
        {
            SongVolumeValue.value = volume;
            EditorDataManager.Instance.SongVolume = volume;
            EditorEventDefine.EventSetSongVolume.SendEventMessage(volume);
        }

        public void SetCoolValue(string value)
        {
            float newValue = string.IsNullOrEmpty(value)
                ? Mathf.Clamp(float.Parse(CoolValue.text), 0, Mathf.Infinity)
                : Mathf.Clamp(float.Parse(value), 0, Mathf.Infinity);
            CoolValue.text = newValue.ToString();
            EditorDataManager.Instance.CoolValue = newValue;
        }
        
        public void SetGreatValue(string value)
        {
            float newValue = string.IsNullOrEmpty(value)
                ? Mathf.Clamp(float.Parse(GreatValue.text), 0, Mathf.Infinity)
                : Mathf.Clamp(float.Parse(value), 0, Mathf.Infinity);
            GreatValue.text = newValue.ToString();
            EditorDataManager.Instance.GreatValue = newValue;
            
        }

        
        public void SetBadValue(string value)
        {
            float newValue = string.IsNullOrEmpty(value)
                ? Mathf.Clamp(float.Parse(BadValue.text), 0, Mathf.Infinity)
                : Mathf.Clamp(float.Parse(value), 0, Mathf.Infinity);
            BadValue.text = newValue.ToString();
            EditorDataManager.Instance.BadValue = newValue;
        }
        
        public void SetMissValue(string value)
        {
            float newValue = string.IsNullOrEmpty(value)
                ? Mathf.Clamp(float.Parse(MissValue.text), 0, Mathf.Infinity)
                : Mathf.Clamp(float.Parse(value), 0, Mathf.Infinity);
            MissValue.text = newValue.ToString();
            EditorDataManager.Instance.MissValue = newValue;
        }


        public void SetDifficulty(int index)
        {
            EditorDataManager.Instance.LevelDifficulty = (LevelDifficulty)index;
            LevelDifficulty.value = index;
        }
        
        #endregion
       
       
    }
}