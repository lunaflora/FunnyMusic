using System;

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
}