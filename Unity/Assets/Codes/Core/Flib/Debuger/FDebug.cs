//==================={By Qcbf|qcbf@qq.com|5/29/2021 1:13:18 PM}===================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

#pragma warning disable CA2211
namespace FLib
{
    public static class FDebug
    {
        public static Action<EType, string> OnOutputEvent;
        public static EOption Options = EOption.AppendDate;

        public static bool IsPrintVerbose => (Options & EOption.EnableVerbose) != 0;

        [Flags]
        public enum EOption
        {
            None = 0,
            AppendDate = 0x1,
            AppendCallStack = 0x2,
            AppendThreadId = 0x4,
            DisablePrint = 0x8,
            DisableWarn = 0x10,
            EnableVerbose = 0x20,
            DisableError = 0x40,
            ErrorToException = 0x80,
        }

        public enum EType : byte
        {
            Print,
            Warn,
            Error,
        }

        public static string GetNowDate()
        {
            return DateTime.Now.ToString("HH:mm:ss.ff");
        }

        /// <summary>
        /// 
        /// </summary>
        public static string Format(object textObj, string tag1 = null, string tag2 = null)
        {
            var text = SlimJson.SerializeToLog(textObj);
            var strbuf = StringFLibUtility.GetStrBuf((tag1?.Length).GetValueOrDefault() + (tag2?.Length).GetValueOrDefault() + text.Length);

            if ((Options & EOption.AppendDate) != 0)
                strbuf.Append('[').Append(GetNowDate()).Append(']');

            if ((Options & EOption.AppendThreadId) != 0)
                strbuf.Append('[').Append(Environment.CurrentManagedThreadId).Append(']');

            var isHaveTag = false;
            if (!string.IsNullOrEmpty(tag1))
            {
                strbuf.Append('[').Append(tag1).Append(']');
                isHaveTag = true;
            }

            if (!string.IsNullOrEmpty(tag2))
            {
                strbuf.Append('[').Append(tag2).Append(']');
                isHaveTag = true;
            }

            if (isHaveTag)
                strbuf.Append(' ');

            strbuf.Append(text);

            if ((Options & EOption.AppendCallStack) != 0)
                strbuf.AppendLine().AppendLine(GetStackTrace(2, isNeedFileInfo: true));

            return StringFLibUtility.ReleaseStrBufAndResult(strbuf);
        }

        [Conditional("DEBUG")]
        public static void Verbose(object arg, string tag1 = null, string tag2 = null)
        {
            if (IsPrintVerbose)
                OnOutputEvent?.Invoke(EType.Print, Format(arg, tag1, tag2));
        }

        [Conditional("DEBUG"), Conditional("TRACE")]
        public static void Print(object arg, string tag1 = null, string tag2 = null)
        {
            if ((Options & EOption.DisablePrint) == 0)
                OnOutputEvent?.Invoke(EType.Print, Format(arg, tag1, tag2));
        }

        [Conditional("DEBUG"), Conditional("TRACE")]
        public static void Warn(object arg, string tag1 = null, string tag2 = null)
        {
            if ((Options & EOption.DisableWarn) == 0)
                OnOutputEvent?.Invoke(EType.Warn, Format(arg, tag1, tag2));
        }

        public static void Error(object arg, string tag1 = null, string tag2 = null)
        {
            if ((Options & EOption.DisableError) == 0)
            {
                OnOutputEvent?.Invoke(EType.Error, Format(arg, tag1, tag2));
                if ((Options & EOption.ErrorToException) != 0)
                    throw new Exception(Format(arg, tag1, tag2));
            }
        }

        [Conditional("DEBUG"), Conditional("TRACE"), AssertionMethod]
        public static void Assert([AssertionCondition(AssertionConditionType.IS_TRUE)] bool condition, object text = null, string tag1 = null, string tag2 = null, EErrorLevel errorLevel = default)
        {
            if (!condition)
                ErrorLevelHelper.Throw(errorLevel, text, tag1, tag2);
        }

        [Conditional("DEBUG"), Conditional("TRACE"), AssertionMethod]
        public static void AssertNotNull([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] object target, object text = null, string tag1 = null, string tag2 = null, EErrorLevel errorLevel = default)
        {
            Assert(target != null, text, tag1, tag2, errorLevel);
        }

#if DEBUG
        public static void TestCodeExecTime(Action action, int count = 1, string tag = "")
        {
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
                action();
            Print($"[{(sw.ElapsedMilliseconds * 0.001f).ToString("f4") + "s"}]", tag);
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        public static string GetStackTrace(int skinFrames = 0, int count = int.MaxValue, string splitChar = "\n", bool isNeedFileInfo = false)
        {
#if TRACE || DEBUG
            var frames = new StackTrace(skinFrames + 1, isNeedFileInfo).GetFrames();
            count = Math.Min(frames!.Length, count);
            var strbuf = StringFLibUtility.GetStrBuf();
            for (var i = 0; i < count; i++)
            {
                var m = frames[i].GetMethod();
                strbuf.Append(m?.DeclaringType).Append('.').Append(m?.Name).Append(':').Append(frames[i].GetFileLineNumber()).Append(splitChar);
            }

            return StringFLibUtility.ReleaseStrBufAndResult(strbuf);
#else
            return String.Empty;
#endif
        }

        /// <summary>
        /// 输出到console 
        /// </summary>
        public static void ConsoleOutputer(EType type, string text)
        {
            Console.WriteLine($"[{type}]{text}");
        }
    }
}
