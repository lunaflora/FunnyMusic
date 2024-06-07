// =================================================={By Qcbf|qcbf@qq.com|2024-1-8}==================================================

// #undef DEBUG

using System;
using System.Diagnostics;

namespace FLib
{
    public struct DebugOnlyString
    {
#if DEBUG
        public string Text;
#endif

        public bool IsEmpty =>
#if DEBUG
            string.IsNullOrWhiteSpace(Text)
#else
            true
#endif
        ;

        public override string ToString()
        {
            return this;
        }

        [Conditional("DEBUG")]
        public static void Concat(ref DebugOnlyString oldStr, string newStr)
        {
            oldStr += newStr;
        }

        [Conditional("DEBUG")]
        public static void ConcatLine(ref DebugOnlyString oldStr, string newStr)
        {
            oldStr += newStr
#if DEBUG
                      + Environment.NewLine
#endif
                ;
        }

        public DebugOnlyString Append(DebugOnlyString newStr)
        {
            Concat(ref this, newStr);
            return this;
        }

        public DebugOnlyString AppendLine(DebugOnlyString newStr)
        {
            ConcatLine(ref this, newStr);
            return this;
        }

        public static implicit operator DebugOnlyString(string v) => new DebugOnlyString
        {
#if DEBUG
            Text = v
#endif
        };

        public static implicit operator string(DebugOnlyString v) =>
#if DEBUG
            v.Text;
#else
            string.Empty;
#endif
    }
}
