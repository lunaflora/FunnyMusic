//-----------------------------------------------------------------------
//| by:Qcbf    qcbf@qq.com                                                      |
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FLib
{
    public static class TypeAssistant
    {
        [NonSerialized]
        public static Assembly[] AllAssemblies = { typeof(TypeAssistant).Assembly };
        public static readonly Dictionary<string, Type> CustomTypeMap = new();


        private static readonly Func<Assembly, string, bool, Type> mTypeFinder = (arg1, arg2, arg3) =>
        {
            switch (arg2)
            {
                case "short": case "System.Int16": return typeof(short);
                case "int": case "System.Int32": return typeof(int);
                case "long": case "System.Int64": return typeof(long);
                case "ushort": case "System.UInt16": return typeof(ushort);
                case "boolean": case "bool": case "System.Boolean": return typeof(bool);
                case "uint": case "System.UInt32": return typeof(uint);
                case "ulong": case "System.UInt64": return typeof(ulong);
                case "byte": case "System.Byte": return typeof(byte);
                case "char": case "System.Char": return typeof(char);
                case "sbyte": case "System.SByte": return typeof(sbyte);
                case "string": case "System.String": return typeof(string);
                case "type": case "System.Type": return typeof(Type);
                case "float": case "System.Single": return typeof(float);
                case "double": case "System.Double": return typeof(double);
                case "list": case "System.Collections.Generic.List`1": return typeof(List<>);
                case "dict": case "System.Collections.Generic.Dictionary`2": return typeof(Dictionary<,>);
                case "object": case "System.Object": return typeof(object);
                default:
                    if (CustomTypeMap.TryGetValue(arg2, out var t))
                    {
                        return t;
                    }
                    else if (AllAssemblies == null || string.IsNullOrEmpty(arg2))
                    {
                        return null;
                    }
                    foreach (var asm in AllAssemblies)
                    {
                        var found = asm.GetType(arg2, false, arg3);
                        if (found != null)
                        {
                            CustomTypeMap.Add(arg2, found);
                            return found;
                        }
                    }
                    return Type.GetType(arg2, false, arg3);
            }
        };



        public static void Clear()
        {
            Array.Resize(ref AllAssemblies, 1);
            CustomTypeMap.Clear();
        }

        /// <summary>
        ///
        /// </summary>
        public static void RemoveAssembly(Assembly target)
        {
            ArrayFLibUtility.Remove(ref AllAssemblies, target);
        }

        /// <summary>
        ///
        /// </summary>
        public static void AddAssemblies(params Assembly[] assemblies)
        {
            var hash = new HashSet<Assembly>(AllAssemblies);
            foreach (var item in assemblies)
                hash.Add(item);
            AllAssemblies = hash.ToArray();
            //var count = AllAssemblies.Length;
            //Array.Resize(ref AllAssemblies, count + assemblies.Length);
            //Array.Copy(assemblies, 0, AllAssemblies, count, assemblies.Length);
        }


        public static void UnregisterCustomFinderType(string name)
        {
            CustomTypeMap.Remove(name);
        }
        public static void RegisterCustomFinderType<T>(string name)
        {
            RegisterCustomFinderType(name ?? typeof(T).FullName, typeof(T));
        }
        public static void RegisterCustomFinderType(string name, Type t)
        {
            CustomTypeMap[name] = t;
        }



        public static T New<T>()
        {
            return Activator.CreateInstance<T>();
        }


        public static object New(Type t)
        {
            return Activator.CreateInstance(t, Array.Empty<object>());
        }


        public static object New(Type t, params object[] args)
        {
            return Activator.CreateInstance(t, args);
        }


        public static object New(string name, bool ignoreCase = false, bool isThrowOnError = true, object[] args = null)
        {
            var type = GetType(name, ignoreCase, isThrowOnError);
            if (type == null)
            {
                return null;
            }
            return Activator.CreateInstance(type, args ?? Array.Empty<object>());
        }

        public static Type GetType(string name, bool ignoreCase = false, bool isThrowOnError = true)
        {
            var result = Type.GetType(name, null, mTypeFinder, false, ignoreCase);
            if (isThrowOnError && result == null)
            {
                throw new TypeLoadException(name);
            }
            return result;
        }

        public static string GetTypeName(Type t)
        {
            if (t == typeof(byte)) return "byte";
            else if (t == typeof(short)) return "short";
            else if (t == typeof(int)) return "int";
            else if (t == typeof(long)) return "long";
            else if (t == typeof(sbyte)) return "sbyte";
            else if (t == typeof(ushort)) return "ushort";
            else if (t == typeof(ulong)) return "ulong";
            else if (t == typeof(string)) return "string";
            else if (t == typeof(char)) return "char";
            //else if (t == typeof(List<>)) return "int`1";
            //else if (t == typeof(Dictionary<,>)) return "int`2";
            else if (t == typeof(float)) return "float";
            else if (t == typeof(double)) return "double";
            else if (t == typeof(Type)) return "type";
            else return t.ToString();
        }



    }
}
