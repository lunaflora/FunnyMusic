using Unity.VisualScripting;
using UnityEngine;

namespace Framework
{
    public static class GameObjectUtil
    {
        public static Transform CreateTransform(string name, Transform parent, bool dontDestroyOnLoad = false)
        {
            Transform create = new GameObject(name).transform;
            create.SetParent(parent);
            if (dontDestroyOnLoad)
            {
                GameObject.DontDestroyOnLoad(create.gameObject);
            }

            return create;
        }
        

        
    }
}