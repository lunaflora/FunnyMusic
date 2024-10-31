using System;
using UnityEngine;

namespace FunnyMusic
{
    public enum TrackType
    {
        LeftTrack,
        RightTrack,
        TrackCount
    }

    public enum BeatType
    {
        Tap,//点击鼓点
        SwipeUp, //向上滑动
        SwipeDown,//向下滑动
        Hold //长按
    }

    public enum MusicPlayState
    {
        Prepare,
        Play,
        End
    }


    /// <summary>
    /// 音频波纹图数据
    /// </summary>
    public class WaveformData
    {
        public int waveformWidth;
        public float maxWaveform;
        public float[] waveformDatas;

    }
    
    
    /// <summary>
    /// Input event data tracks the input type.
    /// </summary>
    public class InputEventData
    {
        public int InputID;
        public int TrackID;
        public Vector2 Direction;

        public virtual bool Tap => InputID == 0;
        public virtual bool Release => InputID == 1;
        public virtual bool Swipe => InputID == 2;

        public InputEventData(int trackID, int inputID)
        {
            TrackID = trackID;
            InputID = inputID;
        }

    }

    /// <summary>
    /// The beat trigger event data is used to hold information about the trigger event.
    /// </summary>
    public class BeatTriggerEventData
    {
    }
}