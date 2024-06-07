using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "GameObjectPoolSetting", menuName = "Settings/GameObjectPoolSetting", order = 2)]
    public class GameObjectPoolSetting : ScriptableObject
    {
        [Header("默认对象失效时间")]
        public float DefaultObjectExpireTime = 60;

        /// <summary>
        /// 默认对象池失效时间
        /// </summary>
        [Header("默认对象池失效时间")]
        public float DefaultPoolExpireTime = 60;

        /// <summary>
        /// 单帧最大实例化数
        /// </summary>
        [Header("单帧最大实例化数")]
        public int MaxInstantiateCount = 10;
    }

}
