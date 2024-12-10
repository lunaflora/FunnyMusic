using FLib;

namespace FunnyMusic
{
    public static class LoggerHelper
    {
        public static void LogPlayer(string message)
        {
            message = string.Format("<color=pink>{0}</color>", message);
            FDebug.Print(message);
        }
    }
}