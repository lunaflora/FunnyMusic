using UnityEngine;

namespace RhythmEditor
{
    public static class AudioHelper
    {

        public static float GetAudioTime(AudioSource audioSource)
        {
            if (audioSource.isPlaying)
            {
                int timeSamples = audioSource.timeSamples;
                float timeSeconds = (float)timeSamples / (float)audioSource.clip.frequency;
                return timeSeconds;
            }

            return 0;
        }
    }
}