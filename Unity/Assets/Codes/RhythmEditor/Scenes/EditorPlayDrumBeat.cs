using UnityEngine;

namespace RhythmEditor
{
    public class EditorPlayDrumBeat : MonoBehaviour
    {
        public DrumBeatData DrumBeatData;
        public DrumBeatSceneData DrumBeatSceneData;

        public Transform BeatStart,BeatEnd;

        public void UpdateDrumBeat(float updateTime = -1)
        {
            float beatTime = DrumBeatData.BeatTime;
            float currentTime = updateTime == -1 ? EditorDataManager.Instance.CurrentTime : updateTime;

            //剩余触发时间 beatTime - currentTime
            float beatPosPercent = Mathf.Clamp((beatTime - currentTime) / DrumBeatSceneData.Float_0, 0, 1);
            Vector3 beatPos = Vector3.Lerp(BeatEnd.position,BeatStart.position, beatPosPercent);
            transform.position = beatPos;
        }

    }
}