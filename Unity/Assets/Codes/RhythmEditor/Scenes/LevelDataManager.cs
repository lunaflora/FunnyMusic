using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using FLib;
using UniFramework.Event;
using UnityEngine;
using UnityEngine.Networking;

namespace RhythmEditor
{
    public class LevelDataManager : MonoBehaviour
    {
        private readonly EventGroup eventGroup = new EventGroup();
        private void OnEnable()
        {
            eventGroup.AddListener<EditorEventDefine.EventUploadMusic>(UploadMusic);
        }

        private void OnDisable()
        {
            eventGroup.RemoveAllListener();
        }

        public void UploadMusic(IEventMessage eventMessage)
        {
            
            EditorEventDefine.EventSetCurrentTime.SendEventMessage(0);
            FilePropertyData openFile = new FilePropertyData();
            
            openFile.structSize = Marshal.SizeOf(openFile);
            openFile.filter = "音频文件(*.mp3,*.wav,*.aif,*.ogg)\0*.mp3;*.wav;*.aif;*.ogg";
            openFile.file = new string(new char[256]);
            openFile.maxFile = openFile.file.Length;
            openFile.fileTitle = new string(new char[64]);
            openFile.maxFileTitle = openFile.fileTitle.Length;
            string path = Application.streamingAssetsPath;
            openFile.initialDir = path;
            openFile.title = "上传音乐";
            openFile.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;

            if (OpenFile(openFile))
            {
                string filePath = "file://" + openFile.file;
                LoadAudioSource(filePath).Forget();
            }

        }

        /// <summary>
        /// 从本地或者网络读取音频文件
        /// </summary>
        /// <param name="filePath"></param>
        private async UniTask LoadAudioSource(string filePath)
        {
            AudioType audioType = AudioType.MPEG;
            if (Regex.Matches(filePath, @".map3$").Count > 0)
            {
                audioType = AudioType.MPEG;
            }
            else if (Regex.Matches(filePath, @".ogg$").Count > 0)
            {
                audioType = AudioType.MPEG;
            }
            else if (Regex.Matches(filePath, @".wav$").Count > 0)
            {
                audioType = AudioType.MPEG;
            }
            else if (Regex.Matches(filePath, @".aif$").Count > 0)
            {
                audioType = AudioType.MPEG;
            }

            using (var uwr = UnityWebRequestMultimedia.GetAudioClip(filePath,audioType))
            {
                await uwr.SendWebRequest();
                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    FDebug.Error($"{uwr.error}");
                    return;
                }

                EditorDataManager.Instance.LoadingAudioPath = filePath;
                EditorDataManager.Instance.LoadingAudio = DownloadHandlerAudioClip.GetContent(uwr);
                EditorEventDefine.EventUploadMusicComplete.SendEventMessage();
            }
            
            
        }
        
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        private static extern bool GetOpenFileName([In, Out] FilePropertyData ofn);
        public static bool OpenFile([In, Out] FilePropertyData ofn)
        {
            return GetOpenFileName(ofn);
        }
    }
}