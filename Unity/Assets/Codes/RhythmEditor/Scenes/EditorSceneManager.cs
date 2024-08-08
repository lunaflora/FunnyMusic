using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RhythmEditor
{
    public class EditorSceneManager : MonoBehaviour
    {
        public int DefaultGroupID = 0;
        public List<ScenesSO> ScenesSoList = new List<ScenesSO>();

        /// <summary>
        /// 编辑器加载界面
        /// </summary>
        public CanvasGroup LoadingUI;

        private void Start()
        {
            
        }

        private void StartLoadSceneGroup(int groupID)
        {
           
        }

        private async UniTask LoadSceneGroup(int groupID)
        {
            while (!Mathf.Approximately(LoadingUI.alpha,1))
            {
                await UniTask.Yield();
            }

            ScenesSO sceneSo = ScenesSoList.Find((sceneSo) => sceneSo.SceneGroupID == groupID);
            if (sceneSo != null)
            {
                //for(int i= SceneManager. )
            }
        }
        
    }
}