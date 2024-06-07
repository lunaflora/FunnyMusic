using UnityEngine;
namespace FLib
{
    public class FLibUnityInitializer
    {
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
#endif
        public static void InitializeSystem()
        {
            FDebug.OnOutputEvent += OnLogOutputEvent;
            
        }

        private static void OnLogOutputEvent(FDebug.EType type, string text)
        {
            if (type == FDebug.EType.Print)
            {
                Debug.Log(text);
            }
            else if (type == FDebug.EType.Warn)
            {
                Debug.LogWarning(text);
            }
            else if (type == FDebug.EType.Error)
            {
                Debug.LogError(text);
            }
        }
    }
}