//==================={By Qcbf|qcbf@qq.com|2/28/2022 4:52:14 PM}===================

using FLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FLib
{
    public abstract class CommandLineHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Execute(object commandHandler, string[] args)
        {
            try
            {
                string methodName = default;
                foreach (var item in args)
                {
                    if (item.Equals("help", StringComparison.InvariantCultureIgnoreCase))
                    {
                        FDebug.Print(GetHelp(commandHandler), nameof(CommandLineHelper));
                    }
                    else if (item[0] == '-')
                    {
                        var splitIndex = item.IndexOf('=');
                        var name = item.Substring(1, splitIndex - 1);
                        var value = item.Substring(splitIndex + 1);
                        var field = commandHandler.GetType().GetField(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (field == null)
                        {
                            throw new Exception("not found field[" + name + ']');
                        }

                        field.SetValue(commandHandler, Convert.ChangeType(value, field.FieldType));
                    }
                    else
                    {
                        methodName = item.ToLowerInvariant();
                    }
                }

                if (methodName == null) throw new Exception("not found method");
                var method = commandHandler.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (method?.IsDefined(typeof(CommentAttribute)) != true) throw new Exception("not found method: " + methodName);
                method.Invoke(commandHandler, null);
            }
            catch (Exception ex)
            {
                FDebug.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static StringBuilder GetHelp(object commandHandler)
        {
            var strbuf = new StringBuilder();
            strbuf.AppendLine("").AppendLine("fields:");
            foreach (var item in commandHandler.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                var comment = item.GetCustomAttribute<CommentAttribute>();
                if (comment != null)
                {
                    strbuf.Append('\t').Append('-').Append(item.Name).Append('\t').Append(comment.Name)
                        .Append('[').Append("Default: ").Append(item.GetValue(commandHandler)).Append(']');
                    strbuf.AppendLine();
                }
            }

            strbuf.AppendLine().AppendLine("methods:");
            foreach (var item in commandHandler.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                var comment = item.GetCustomAttribute<CommentAttribute>();
                if (comment != null)
                {
                    strbuf.Append('\t').Append('-').Append(item.Name).Append('\t').Append(comment.Name);
                    strbuf.AppendLine();
                }
            }

            return strbuf;
        }
    }
}
