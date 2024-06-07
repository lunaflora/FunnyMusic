// =================================================={By Qcbf|qcbf@qq.com|2024-05-14}==================================================

using System;

namespace FLib
{
    public enum EErrorLevel : byte
    {
        Exception,
        Error,
        Warn,
        Print,
        Silence,
    }

    public static class ErrorLevelHelper
    {
        public static void Throw(in EErrorLevel Level, in object log, in string tag1 = null, in string tag2 = null)
        {
            if (Level == EErrorLevel.Exception)
                throw new Exception(FDebug.Format(log, tag1, tag2));
            if (Level == EErrorLevel.Error)
                FDebug.Error(log, tag1, tag2);
            else if (Level == EErrorLevel.Warn)
                FDebug.Warn(log, tag1, tag2);
            else if (Level == EErrorLevel.Print)
                FDebug.Print(log, tag1, tag2);
        }
    }
}
