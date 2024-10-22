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
            BeatConfigCategory beatConfigCategory = ConfigGenerateComponent.Instance.AllConfig[typeof(BeatConfigCategory)] as BeatConfigCategory;
            List<BeatConfig> beatConfigs =  beatConfigCategory.GetList().OfType<BeatConfig>().ToList();


            BeatConfig beatConfig =
                beatConfigs.Find((config) => config.BeatType == beatType && config.TrackID == trackID);

            return beatConfig;

        }

        #endregion


        #region CommonFunction

        public static async UniTask<GameObject> SpawnDrumBeat(string prefab,int id)
        {
            UniTaskCompletionSource<GameObject> tcs = new UniTaskCompletionSource<GameObject>();

            GameObject drumBeat =
                await GameObjectPoolManager.GetAsync(string.Format(ResourcesPath.InternalDrumBeatPath, prefab));

            drumBeat.GetComponent<DrumBeatBehaviour>().BeatID = id;
            tcs.TrySetResult(drumBeat);

            return await tcs.Task;
        }
        

        #endregion
    }
}