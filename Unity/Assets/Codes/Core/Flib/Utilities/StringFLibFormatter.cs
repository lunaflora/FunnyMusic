//==================={By Qcbf|qcbf@qq.com|12/10/2022 4:30:36 PM}===================

using System;
using System.Collections.Generic;
using FLib;

namespace FLib
{
    public class StringFLibFormatter : IFormatProvider, ICustomFormatter
    {
        public static StringFLibFormatter Main = new();

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (format?.StartsWith('%') == true)
            {
                var baseValue = 1f;
                for (var i = 1; i < format.Length; i++)
                {
                    if (format[i] == '%') baseValue *= 0.1f;
                    else break;
                }
                return (Convert.ToSingle(arg) * baseValue).ToString("0.##") + "%";
            }
            else if (format?.StartsWith('/') == true)
            {
                return (Convert.ToSingle(arg) / format[1..].AsSpan().ToFloat()).ToString("0.##");
            }
            else if (format?.StartsWith('*') == true)
            {
                return (Convert.ToSingle(arg) * format[1..].AsSpan().ToFloat()).ToString("0.##");
            }
            else if (format == "abs")
            {
                return Math.Abs(Convert.ToSingle(arg)).ToString();
            }
            return arg.ToString();
        }

        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }
    }
}
