using UnityEngine;

namespace Framework
{
    public enum LoggerLevel
    {
        Log,
        Warning,
        Error
            
    }
    public static class CustomLogger
    {

        /// <summary>
        /// 先占位，后面统一替换
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public static void Log(LoggerLevel level, string message)
        {
            switch (level)
            {
                case LoggerLevel.Error:
                    Debug.LogError(message);
                    break;
                case LoggerLevel.Warning:
                    Debug.LogWarning(message);
                    break;
                default:
                    Debug.Log(message);
                    break;
                
            }
        }
        
    }
}