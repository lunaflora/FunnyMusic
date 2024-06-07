using UnityEngine;

namespace Core
{
    public static class GameObjectUtil
    {
        public static T GetOrAddComponent<T>(this GameObject uo) where T : Component
        {
            return uo.GetComponent<T>() ?? uo.AddComponent<T>();
        }
        
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