using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace Core
{
    /// <summary>
    /// 资源绑定器
    /// </summary>
    public class AssetBinder : MonoBehaviour
    {
        public readonly List<AssetHandle> Handlers = new List<AssetHandle>();

        /// <summary>
        /// 绑定句柄
        /// </summary>
        public void BindTo(AssetHandle handler)
        {
            
            Handlers.Add(handler);
        }
        

        private void OnDestroy()
        {
            foreach (AssetHandle handler in Handlers)
            {
                handler.Release();
            }
            Handlers.Clear();

        }
    }
}