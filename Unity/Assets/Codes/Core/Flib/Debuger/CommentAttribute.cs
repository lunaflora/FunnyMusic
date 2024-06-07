//==================={By Qcbf|qcbf@qq.com|5/30/2021 4:06:17 PM}===================

using System;
using System.Diagnostics;
using System.Reflection;

namespace FLib
{
    [Conditional("DEBUG"), AttributeUsage(AttributeTargets.All)]
    public class CommentAttribute : Attribute
    {
        public string Name;
        public string Detail;

        public CommentAttribute(string name, string detail = "")
        {
            Name = name;
            Detail = detail;
        }

        public override string ToString() => ToString(string.Empty);
        public string ToString(string additionText) => string.IsNullOrWhiteSpace(Detail) ? Name + additionText : Name + "\n" + Detail;

        public static string TryGetLabel(Type type, string additionText = "")
        {
            if (type == null)
                return string.Empty;
            var comment = type.GetCustomAttribute<CommentAttribute>();
            return comment?.ToString(additionText) ?? type.Name;
        }
    }
}
