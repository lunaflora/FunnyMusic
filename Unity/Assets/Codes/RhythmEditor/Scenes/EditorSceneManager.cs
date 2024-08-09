using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NodeCanvas.Tasks.Actions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RhythmEditor
{
    /// <summary>
    /// 制谱器场景管理
    /// </summary>
    public class EditorSceneManager : MonoBehaviour
    {
        public int DefaultGroupID = 0;
        public List<ScenesSO> ScenesSoList = new List<ScenesSO>();

        /// <summary>
        /// 编辑器加载界面
        /// </summary>
        public CanvasGroup LoadingUI;

       
        private void Awake()
        {
            Entry();
            StartLoadSceneGroup(DefaultGroupID);
        }
        

        /// <summary>
        /// 制谱器入口
        /// 初始化各种数据
        /// </summary>
        private void Entry()
        {
            
        }

        private void StartLoadSceneGroup(int groupID)
        {
            LoadSceneGroup(groupID).Forget();
        }

        /// <summary>
        /// 加载制谱器场景
        /// </summary>
        /// <param name="groupID"></param>
        private async UniTask LoadSceneGroup(int groupID)
        {
            
            ScenesSO sceneSo = ScenesSoList.Find((sceneSo) => sceneSo.SceneGroupID == groupID);
            if (sceneSo != null)
            {
                for (int i = 0; i < sceneSo.SceneNameList.Count; i++)
                {
                    await SceneManager.UnloadSceneAsync(sceneSo.SceneNameList[i]);
                }
                
                string loadSceneName = string.Empty;
                List<AsyncOperation> loadOperation = new List<AsyncOperation>();
                for (int i = 0; i < sceneSo.SceneNameList.Count; i++)
                {
                    loadSceneName = sceneSo.SceneNameList[i];
                    loadOperation.Add(SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Additive));
                    
                }

                bool allComplete = loadOperation.FindAll((op) => op.isDone == true).Count ==
                                   sceneSo.SceneNameList.Count;

                while (!allComplete)
                {
                    await UniTask.Yield();
                    allComplete = loadOperation.FindAll((op) => op.isDone == true).Count ==
                                  sceneSo.SceneNameList.Count;
                    LoadingUI.alpha = Mathf.MoveTowards(LoadingUI.alpha, 0, Time.deltaTime * 1f);
                }

                LoadingUI.alpha = 0;
            }
        }
        
    }
}