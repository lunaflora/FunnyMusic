using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using Object = UnityEngine.Object;

namespace Core
{
    /// <summary>
    /// 用Unitask封装YooAsset相关资源管理API
    /// </summary>
    public class AssetLoaderSystem : Singleton<AssetLoaderSystem>
    {
        private static ResourcePackage defaultPackage;

        public Dictionary<string, AssetHandle> AssetsOperationHandles = new Dictionary<string, AssetHandle>(100);

        public Dictionary<string, SubAssetsHandle> SubAssetsOperationHandles = new Dictionary<string, SubAssetsHandle>();

        public Dictionary<string, RawFileHandle> RawFileOperationHandles = new Dictionary<string, RawFileHandle>(100);

        public Dictionary<HandleBase, Action<float>> HandleProgresses = new Dictionary<HandleBase, Action<float>>();

        public Queue<HandleBase> DoneHandleQueue = new Queue<HandleBase>();

        public async UniTask Initialize()
        {
            //1.读取全局配置
            AssetSetting assetSetting = Resources.Load<AssetSetting>("Settings/AssetSetting");

            //2.初始化资源系统
            YooAssets.Initialize();

            YooAssets.SetOperationSystemMaxTimeSlice(30);

            // 开始补丁更新流程
            PatchOperation operation = new PatchOperation(assetSetting.PackageName,
                EDefaultBuildPipeline.ScriptableBuildPipeline.ToString(),
                assetSetting.PlayMode);
            YooAssets.StartOperation(operation);

            await operation.ToUniTask();

            defaultPackage = YooAssets.TryGetPackage(assetSetting.PackageName);

            if (defaultPackage == null)
            {
                defaultPackage = YooAssets.CreatePackage(assetSetting.PackageName);
                YooAssets.SetDefaultPackage(defaultPackage);
            }
        }

        /// <summary>
        /// 加载进度的更新
        /// </summary>
        public void Update()
        {
            foreach (var kv in HandleProgresses)
            {
                HandleBase handle = kv.Key;
                Action<float> progress = kv.Value;

                if (!handle.IsValid)
                {
                    continue;
                }

                if (handle.IsDone)
                {
                    DoneHandleQueue.Enqueue(handle);
                    progress?.Invoke(1);
                    continue;
                }

                progress?.Invoke(handle.Progress);
            }

            while (DoneHandleQueue.Count > 0)
            {
                var handle = DoneHandleQueue.Dequeue();
                HandleProgresses.Remove(handle);
            }
        }


        public override void Destroy()
        {
            ForceUnloadAllAssets();
            AssetsOperationHandles.Clear();
            SubAssetsOperationHandles.Clear();
            RawFileOperationHandles.Clear();
            HandleProgresses.Clear();
            DoneHandleQueue.Clear();

            base.Destroy();
        }


        #region AssetLoad 绑定资源到GameObject 自动释放
        /// <param name="location"></param> Assets/Res/Prefabs/TestImg.prefab
        /// <param name="assetBinder"></param> 通过绑定资源句柄到GameObject实现自动销毁
        /// <returns></returns>
        public async UniTask<TObject> LoadAssetAsync<TObject>(string location, GameObject assetBinder,
            uint priority = 0,
            IProgress<float> progress = null,
            PlayerLoopTiming timing = PlayerLoopTiming.Update) where TObject : Object
        {
            UniTaskCompletionSource<TObject> tsc = new UniTaskCompletionSource<TObject>();

            AssetHandle handle = defaultPackage.LoadAssetAsync<TObject>(location,priority);
            await handle.ToUniTask(progress, timing);

            BindToGameObject(assetBinder, handle);

            var obj = handle.AssetObject as TObject;

            tsc.TrySetResult(obj);

            return await tsc.Task;
        }

        public async UniTask<GameObject> InstantiateSync(string location, Transform parent = null,
            uint priority = 0,
            IProgress<float> progress = null,
            PlayerLoopTiming timing = PlayerLoopTiming.Update)
        {
            UniTaskCompletionSource<GameObject> tsc = new UniTaskCompletionSource<GameObject>();

            AssetHandle handle = defaultPackage.LoadAssetAsync<GameObject>(location,priority);
            await handle.ToUniTask(progress, timing);


            var obj = handle.GetAssetObject<GameObject>();

            GameObject gameObject = handle.InstantiateSync(parent);
            BindToGameObject(gameObject, handle);

            tsc.TrySetResult(gameObject);


            return await tsc.Task;
        }


        public async UniTask<GameObject> InstantiateAsync(string location, Transform parent = null,
            uint priority = 0,
            IProgress<float> progress = null,
            PlayerLoopTiming timing = PlayerLoopTiming.Update, bool isAsyncInstantiate = false)
        {
            var loaded = await LoadAssetAsyncHandle<GameObject>(location,priority, progress, timing);
            var gameObj = (GameObject)loaded.AssetObject;
            if (isAsyncInstantiate)
            {
                var complation = AutoResetUniTaskCompletionSource<GameObject>.Create();
                var asyncInstantiateOperation = Object.InstantiateAsync(gameObj, parent);
                asyncInstantiateOperation.completed += _ => complation.TrySetResult(asyncInstantiateOperation.Result[0]);
                gameObj = await complation.Task;
            }
            else
            {
                gameObj = Object.Instantiate(gameObj, parent);
            }

            BindToGameObject(gameObj, loaded);

            return gameObj;
        }

        #region AssetLoad 保存资源句柄 通过资源路径手动释放
        public async UniTask<TObject> LoadAssetAsync<TObject>(string location, uint priority = 0,
            IProgress<float> progress = null,
            PlayerLoopTiming timing = PlayerLoopTiming.Update) where TObject : Object
        {
            UniTaskCompletionSource<TObject> tsc = new UniTaskCompletionSource<TObject>();

            AssetsOperationHandles.TryGetValue(location, out AssetHandle handle);
            if (handle == null)
            {
                handle = defaultPackage.LoadAssetAsync<TObject>(location);
                AssetsOperationHandles[location] = handle;
            }


            await handle.ToUniTask(progress, timing);

            tsc.TrySetResult(handle.GetAssetObject<TObject>());

            return await tsc.Task;
        }

        /// <summary>
        /// 返回资源句柄，外部释放资源句柄
        /// </summary>
        /// <returns></returns>
        public async UniTask<AssetHandle> LoadAssetAsyncHandle<TObject>(string location,uint priority = 0,
            IProgress<float> progress = null,
            PlayerLoopTiming timing = PlayerLoopTiming.Update) where TObject : Object
        {
            UniTaskCompletionSource<AssetHandle> tsc = new UniTaskCompletionSource<AssetHandle>();


            AssetHandle handle = defaultPackage.LoadAssetAsync<TObject>(location,priority);
            await handle.ToUniTask(progress, timing);

            tsc.TrySetResult(handle);

            return await tsc.Task;
        }

        public async UniTask<Scene> LoadAssetAsync(string location, uint priority = 0,
            Action<float> progressCallback = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true,
            PlayerLoopTiming timing = PlayerLoopTiming.Update)
        {
            SceneHandle handle = defaultPackage.LoadSceneAsync(location, loadSceneMode, activateOnLoad, priority);

            if (progressCallback != null)
            {
                HandleProgresses.Add(handle, progressCallback);
            }

            await handle;

            return handle.SceneObject;
        }

        public TObject LoadAssetSync<TObject>(string location) where TObject : Object
        {
            AssetsOperationHandles.TryGetValue(location, out AssetHandle handle);
            if (handle == null)
            {
                handle = defaultPackage.LoadAssetSync<TObject>(location);
                AssetsOperationHandles[location] = handle;
            }

            return handle.GetAssetObject<TObject>();
        }

        public async UniTask<byte[]> LoadRawFileDataAsync(string location)
        {
            if (!RawFileOperationHandles.TryGetValue(location, out RawFileHandle handle))
            {
                handle = defaultPackage.LoadRawFileAsync(location);
                RawFileOperationHandles[location] = handle;
            }

            await handle;

            return handle.GetRawFileData();
        }

        public byte[] LoadRawFileDataSync(string location)
        {
            if (!RawFileOperationHandles.TryGetValue(location, out RawFileHandle handle))
            {
                handle = defaultPackage.LoadRawFileSync(location);
                RawFileOperationHandles[location] = handle;
            }

            return handle.GetRawFileData();
        }


        public async UniTask<string> LoadRawFileTextAsync(string location)
        {
            if (!RawFileOperationHandles.TryGetValue(location, out RawFileHandle handle))
            {
                handle = defaultPackage.LoadRawFileAsync(location);
                RawFileOperationHandles[location] = handle;
            }

            await handle;

            return handle.GetRawFileText();
        }
        #endregion
        #endregion

        #region AssetUnload
        public void UnloadAsset(string location)
        {
            if (AssetsOperationHandles.TryGetValue(location, out AssetHandle assetOperationHandle))
            {
                assetOperationHandle.Release();
                AssetsOperationHandles.Remove(location);
            }
            else if (RawFileOperationHandles.TryGetValue(location, out RawFileHandle rawFileOperationHandle))
            {
                rawFileOperationHandle.Release();
                RawFileOperationHandles.Remove(location);
            }
            else if (SubAssetsOperationHandles.TryGetValue(location, out SubAssetsHandle subAssetsOperationHandle))
            {
                subAssetsOperationHandle.Release();
                SubAssetsOperationHandles.Remove(location);
            }
            else
            {
                FDebug.Error($"资源{location}不存在");
            }
        }


        // 卸载所有引用计数为零的资源包。
        // 可以在切换场景之后调用资源释放方法或者写定时器间隔时间去释放。
        public void UnloadUnusedAssets()
        {
            defaultPackage.UnloadUnusedAssets();
        }


        // 尝试卸载指定的资源对象
        // 注意：如果该资源还在被使用，该方法会无效。
        public void TryUnloadUnusedAsset(string location)
        {
            defaultPackage.TryUnloadUnusedAsset(location);
        }


        // 强制卸载所有资源包，该方法请在合适的时机调用。
        // 注意：Package在销毁的时候也会自动调用该方法。
        public void ForceUnloadAllAssets()
        {
            defaultPackage.ForceUnloadAllAssets();
        }
        #endregion

        /// <summary>
        /// 将资源句柄绑定到游戏物体上，会在指定游戏物体销毁时卸载绑定的资源
        /// </summary>
        public void BindToGameObject(GameObject target, AssetHandle handler)
        {
            if (target == null || handler == null)
            {
                return;
            }

            AssetBinder assetBinder = target.GetOrAddComponent<AssetBinder>();
            assetBinder.BindTo(handler);
        }

        public string[] GetAddressesByTag(string tag)
        {
            AssetInfo[] assetInfos = defaultPackage.GetAssetInfos(tag);
            string[] addresses = new string[assetInfos.Length];
            for (int i = 0; i < assetInfos.Length; i++)
            {
                addresses[i] = assetInfos[i].AssetPath;
            }

            return addresses;
        }
    }
}
