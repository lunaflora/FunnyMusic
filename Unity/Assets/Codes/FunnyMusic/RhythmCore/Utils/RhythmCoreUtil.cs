using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;

namespace FunnyMusic
{
    public static class RhythmCoreUtil
    {
        #region 编辑时到运行时的转换

        /// <summary>
        /// 
        /// </summary>
        public static int BeatTypeToTrackID(int beatType)
        {
            return beatType + 1;
        }

        public static BeatConfig GetBeatConfigByTypeTrack(int beatType,int trackID)
        {

            BeatConfig beatConfig =
                ConfigManager.Instance.BeatConfigs.Find((config) => config.BeatType == beatType && config.TrackID == trackID);

            return beatConfig;

        }

        #endregion


        #region CommonFunction

        public static async UniTask<GameObject> SpawnDrumBeat(string prefab,int id)
        {
            UniTaskCompletionSource<GameObject> tcs = new UniTaskCompletionSource<GameObject>();

            GameObject drumBeat =
                await GameObjectPoolManager.GetAsync(prefab);

            drumBeat.GetComponent<DrumBeatBehaviour>().BeatID = id;
            tcs.TrySetResult(drumBeat);

            return await tcs.Task;
        }

        public static void DespawnDrumBeat(string prefab,GameObject beat)
        {
            GameObjectPoolManager.Release(prefab,beat);
        }
        

        #endregion


        #region Convert

        public static float ConvertIntToFloat(int intValue)
        {
            float floatValue = intValue / (10000.0f);
            return floatValue;
        }
        

        #endregion
    }
}