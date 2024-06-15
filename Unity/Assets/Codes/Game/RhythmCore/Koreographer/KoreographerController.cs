using FLib;
using SonicBloom.Koreo;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 注册音轨事件，处理事件逻辑
    /// </summary>
    public class KoreographerController : MonoBehaviour
    {
        public string EventID = "Finger";
        public void Start()
        {
            Koreographer.Instance.RegisterForEventsWithTime(EventID, NoteSpawn);
        }

        protected void NoteSpawn(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
        {
            FDebug.Print($"NoteSpawn Event : {evt.StartSample}");
        }
    }
}