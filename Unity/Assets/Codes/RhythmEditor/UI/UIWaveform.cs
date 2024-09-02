using System;
using UniFramework.Event;
using UnityEngine;
using UnityEngine.UI;

namespace RhythmEditor
{
    /// <summary>
    /// 波形图的绘制
    /// </summary>
    public class UIWaveform : MonoBehaviour
    {
        public Image WaveformView;
        public Color WaveformColor;
        public Color WaveformClearColor;
        private readonly EventGroup eventGroup = new EventGroup();

        private void Initialize()
        {
            InitWaveformDatas();
         
        }

        private void InitWaveformDatas()
        {
            AudioClip audioClip = EditorDataManager.Instance.LoadingAudio;
            int waveformWidth = Mathf.CeilToInt(audioClip.length * UIConstValue.UIWidthScale);
            //每组数据容量
            int groupDataCounts = audioClip.frequency / (int)UIConstValue.UIWidthScale;
            //歌曲采样数据
            float[] clipSampleData = new float[audioClip.samples * audioClip.channels];
            audioClip.GetData(clipSampleData, 0);
            //波形数值
            float[] waveformDatas = new float[clipSampleData.Length / groupDataCounts];
            float maxWaveform = 0;
            for (int i = 0; i < waveformDatas.Length; i++)
            {
                waveformDatas[i] = 0;
                for (int j = 0; j < groupDataCounts; j++)
                {
                    waveformDatas[i] += Mathf.Abs(clipSampleData[i * groupDataCounts + j]);
                }

                if (maxWaveform < waveformDatas[i])
                {
                    maxWaveform = waveformDatas[i];
                }
                
            }

            DrawWaveformTexture(waveformWidth, waveformDatas, maxWaveform);
        }

        private void DrawWaveformTexture(int waveformWidth,float[] waveformDatas, float maxWaveform)
        {
            Texture2D waveformTexture = new Texture2D(waveformWidth, 250);
            Color[] colors = new Color[waveformWidth * 250];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = WaveformClearColor;
            }
            waveformTexture.SetPixels(colors);

            float horizontalScale = (float)(waveformWidth) / (float)(waveformDatas.Length);
            float verticalScale = 1.0f;
            int halfHeight = (int)(250f / 2f);
            if (maxWaveform > halfHeight)
            {
                verticalScale = halfHeight / maxWaveform;
            }

            for (int i = 0; i < waveformDatas.Length; i++)
            {
                int x = (int)(i * horizontalScale);
                int waveformLength = (int)(waveformDatas[i] * verticalScale);
                int startPos = halfHeight - waveformLength;
                int endPos = halfHeight + waveformLength;

                for (int y = startPos; y <= endPos; y++)
                {
                    waveformTexture.SetPixel(x,y,WaveformColor);
                }
            }
            
            waveformTexture.Apply();
            WaveformView.sprite = Sprite.Create(waveformTexture, new Rect(0, 0, waveformTexture.width, waveformTexture.height),
                new Vector2(0.5f, 0.5f));
        }


        private void UploadMusicComplete(IEventMessage eventMessage)
        {
            Initialize();
        }

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
    }
}