using System;
using System.Collections.Generic;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using FLib;
using UnityEngine;
using YooAsset;
using Object = UnityEngine.Object;

namespace Game
{
    /// <summary>
    /// 游戏对象池管理器
    /// </summary>
    public static partial class GameObjectPoolManager
    {
        private struct InstantiateParam
        {
            public InstantiateHandler Handler;
            public object Userdata;
            public Action<GameObject, object> Callback;
        }

        /// <summary>
        /// 预制体名字->加载好的预制体
        /// </summary>
        private static Dictionary<string, GameObject> loadedPrefabDict = new Dictionary<string, GameObject>();

        /// <summary>
        /// 模板->对象池
        /// </summary>
        internal static Dictionary<GameObject, GameObjectPool> PoolDict = new Dictionary<GameObject, GameObjectPool>();


        /// <summary>
        /// 游戏对象池管理器的根节点
        /// </summary>
        public static Transform Root;

        /// <summary>
        /// 默认对象失效时间
        /// </summary>
        public static float DefaultObjectExpireTime = 60;

        /// <summary>
        /// 默认对象池失效时间
        /// </summary>
        public static float DefaultPoolExpireTime = 120;

        /// <summary>
        /// 单帧最大实例化数
        /// </summary>
        public static int MaxInstantiateCount = 10;

        /// <summary>
        /// 单帧实例化计数器
        /// </summary>
        private static int instantiateCounter;

        /// <summary>
        /// 等待实例化的游戏对象队列
        /// </summary>
        private static Queue<InstantiateParam> handlerQueue = new Queue<InstantiateParam>();

        /// <summary>
        /// 等待卸载的预制体名字列表
        /// </summary>
        private static List<string> waitUnloadPrefabNames = new List<string>();

        private static List<GameObject> tempList = new List<GameObject>();
        
        /// <summary>
        /// 轮询管理器
        /// </summary>
        public static void Update(float deltaTime)
        {

            foreach (var pair in PoolDict)
            {
                //轮询对象池
                pair.Value.OnUpdate(deltaTime);
            }

            //销毁长时间未使用的，且是由管理器加载了预制体资源的对象池
            foreach (KeyValuePair<string, GameObject> pair in loadedPrefabDict)
            {
                GameObjectPool pool = PoolDict[pair.Value];
                if (pool.UnusedTimer > pool.PoolExpireTime)
                {
                    waitUnloadPrefabNames.Add(pair.Key);
                }
            }
            foreach (string prefabName in waitUnloadPrefabNames)
            {
                DestroyPool(prefabName);
            }
            waitUnloadPrefabNames.Clear();

            //处理分帧实例化
            while (instantiateCounter < MaxInstantiateCount && handlerQueue.Count > 0)
            {

                InstantiateParam param = handlerQueue.Dequeue();
                GameObject instance = null;
                if (!param.Handler.IsTokenCanceled)
                {
                    //未被取消才会实例化游戏对象
                    instance = Object.Instantiate(param.Handler.Template, param.Handler.Parent);
                    param.Callback?.Invoke(instance,param.Userdata);
                    instantiateCounter++;
                }
                param.Handler.SetInstance(instance);
            }

            instantiateCounter = 0;
        }
        
        /// <summary>
        /// 获取对象池，若不存在则创建
        /// </summary>
        private static GameObjectPool GetOrCreatePool(GameObject template)
        {
            if (!PoolDict.TryGetValue(template,out var pool))
            {
                GameObject root = new GameObject($"Pool-{template.name}");
                root.transform.SetParent(Root);

                pool = new GameObjectPool(template, root.transform,DefaultPoolExpireTime,DefaultObjectExpireTime);
                PoolDict.Add(template,pool);
            }

            return pool;
        }
        
        /// <summary>
        /// 使用资源名异步创建对象池，此方法创建的对象池会自动销毁
        /// </summary>
        public static async
#if !UNITASK
            Task<bool>
#else
            UniTask<bool>
#endif

            CreatePoolAsync(string assetName, CancellationToken token = default, CanceledCallback onCanceled = null)
        {
            if (loadedPrefabDict.ContainsKey(assetName))
            {
                //对象池已存在
                return true;
            }

            AssetHandle assetHandle = await AssetLoaderSystem.Instance.LoadAssetAsyncHandle<GameObject>(assetName,0,null, PlayerLoopTiming.Update);

            if (assetHandle.Status == EOperationStatus.Failed) 
            {
                FDebug.Error($"对象池异步创建失败：{assetName}");
                return false;
            }
            GameObject prefab = assetHandle.GetAssetObject<GameObject>();
            
            loadedPrefabDict[assetName] = prefab;

            //创建对象池
            GameObjectPool pool = GetOrCreatePool(prefab);
            
            AssetLoaderSystem.Instance.BindToGameObject(pool.Root.gameObject,assetHandle);
            
            return true;

        }
        /// <summary>
        /// 使用模板同步创建对象池，此方法创建的对象池需要由创建者销毁
        /// </summary>
        public static void CreatePool(GameObject template)
        {
            GetOrCreatePool(template);
        }

        /// <summary>
        /// 使用资源名销毁对象池
        /// </summary>
        public static void DestroyPool(string assetName)
        {
            if (!loadedPrefabDict.TryGetValue(assetName,out var prefab))
            {
                return;
            }
            
            loadedPrefabDict.Remove(assetName);
            DestroyPool(prefab);
        }
        
        
        /// <summary>
        /// 使用模板销毁对象池
        /// </summary>
        public static void DestroyPool(GameObject template)
        {
            var pool = GetOrCreatePool(template);
            pool.OnDestroy();
            PoolDict.Remove(template);
        }
        
        /// <summary>
        /// 使用资源名异步获取游戏对象
        /// </summary>
        public static async
#if !UNITASK
            Task<GameObject>
#else
            UniTask<GameObject>
#endif

            GetAsync(string assetName, Transform parent = null,
                CancellationToken token = default, CanceledCallback onCanceled = null)
        {
            GameObject go = null;
            if (loadedPrefabDict.TryGetValue(assetName, out var prefab))
            {
                //此对象池已存在
                go = await GetAsync(prefab, parent, token);
                return go;
            }

            bool success = await CreatePoolAsync(assetName, token, onCanceled);
            if (!success)
            {
                return null;
            }

            prefab = loadedPrefabDict[assetName];

            go = await GetAsync(prefab, parent, token, onCanceled);
            return go;
        }

        /// <summary>
        /// 使用模板异步获取游戏对象
        /// </summary>
        public static async
#if !UNITASK
            Task<GameObject>
#else
            UniTask<GameObject>
#endif

            GetAsync(GameObject template, Transform parent = null,
                CancellationToken token = default, CanceledCallback onCanceled = null)
        {
            var pool = GetOrCreatePool(template);
            var handler = pool.GetAsync(parent, token);
            handler.OnCanceled += onCanceled;
            await handler;
            return handler.Instance;
        }
        
        /// <summary>
        /// 使用资源名同步获取游戏对象，此方法需要先保证对象池已创建
        /// </summary>
        public static GameObject Get(string assetName,Transform parent = null)
        {
            if (!loadedPrefabDict.TryGetValue(assetName,out var prefab))
            {
                Debug.LogError($"使用资源名同步获取游戏对象的对象池不存在，需要先创建:{assetName}");
                return null;
            }

            return Get(prefab, parent);
        }

        /// <summary>
        /// 使用模板同步获取游戏对象
        /// </summary>
        public static GameObject Get(GameObject template, Transform parent = null)
        {
            var pool = GetOrCreatePool(template);
            GameObject go = pool.Get(parent);
            return go;
        }

        
        /// <summary>
        /// 使用资源名释放游戏对象
        /// </summary>
        public static void Release(string assetName, GameObject go)
        {
            if (!loadedPrefabDict.TryGetValue(assetName,out var prefab))
            {
                Debug.LogWarning($"要释放游戏对象的对象池不存在：{assetName}");
                return;
            }

            Release(prefab,go);
        }
        
        /// <summary>
        /// 使用模板释放游戏对象
        /// </summary>
        public static void Release(GameObject template, GameObject go)
        {
            if (!PoolDict.TryGetValue(template, out var pool))
            {
                Debug.LogWarning($"要释放游戏对象的对象池不存在：{go.name}");
                return;
            }
            pool.Release(go);
        }


        /// <summary>
        /// 锁定游戏对象，被锁定后不会被销毁
        /// </summary>
        public static void LockGameObject(string assetName, GameObject go, bool isLock = true)
        {
            if (!loadedPrefabDict.TryGetValue(assetName,out var prefab))
            {
                return;
            }
            LockGameObject(prefab,go,isLock);
        }
        
        /// <summary>
        /// 锁定游戏对象，被锁定后不会被销毁
        /// </summary>
        public static void LockGameObject(GameObject template, GameObject go, bool isLock = true)
        {
            if (!PoolDict.TryGetValue(template, out var pool))
            {
                return;
            }
            pool.LockGameObject(go,isLock);
        }
        
        /// <summary>
        /// 设置对象池的失效时间
        /// </summary>
        public static void SetExpireTime(string assetName, float poolExpireTime,float objExpireTime)
        {
            if (!loadedPrefabDict.TryGetValue(assetName,out var prefab))
            {
                return;
            }
            SetExpireTime(prefab,poolExpireTime,objExpireTime);
        }
          
        /// <summary>
        /// 设置对象池的失效时间
        /// </summary>
        public static void SetExpireTime(GameObject template, float poolExpireTime,float objExpireTime)
        {
            if (!PoolDict.TryGetValue(template, out var pool))
            {
                return;
            }

            pool.PoolExpireTime = poolExpireTime;
            pool.ObjExpireTime = objExpireTime;
        }

        
        /// <summary>
        /// 分帧异步实例化
        /// </summary>
        internal static void InstantiateAsync(InstantiateHandler handler,object userdata,Action<GameObject,object> callback)
        {
            InstantiateParam param = new InstantiateParam();
            param.Handler = handler;
            param.Userdata = userdata;
            param.Callback = callback;
            handlerQueue.Enqueue(param);
        }
        /// <summary>
        /// 使用资源名异步预热对象
        /// </summary>
        public static async
#if !UNITASK
            Task
#else
            UniTask
#endif
            PrewarmAsync(string assetName, int count, CancellationToken token = default, CanceledCallback onCanceled = null)
        {
            bool hasPool = loadedPrefabDict.TryGetValue(assetName, out var prefab);

            void Prewarm(GameObject template)
            {
                var pool = GetOrCreatePool(template);
                for (int i = 0; i < count; i++)
                {
                    GameObject go = pool.Get(Root);
                    tempList.Add(go);
                }

                //预热完毕 统一释放
                foreach (GameObject go in tempList)
                {
                    Release(template, go);
                }

                tempList.Clear();
            }

            if (!hasPool)
            {
                bool success = await CreatePoolAsync(assetName, token, onCanceled);
                if (success)
                {
                    Prewarm(loadedPrefabDict[assetName]);
                }
            }
            else
            {
                Prewarm(prefab);
            }
        }
        
        
    }
}