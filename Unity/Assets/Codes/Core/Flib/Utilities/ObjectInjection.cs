//==================={By Qcbf|qcbf@qq.com|9/7/2023 10:05:56 AM}===================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace FLib
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
    public class ObjectInjectToAttribute : Attribute
    {
        public string Name;
        public string StrParam;
        public int IntParam;

        public ObjectInjectToAttribute(string name = null)
        {
            Name = name;
        }
    }

    /// <summary>
    /// <example>static ReceiveInjection(List&lt;(object type, ObjectInjectToAttribute attr)&gt; list){ /*type is ClassType or MethodInfo*/ }</example>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ObjectInjectionReceiverAttribute : Attribute
    {
        public string Name;
        public string ReceiveInjectMethodName;

        /// <summary>
        /// <example>static ReceiveInjection(List&lt;(object type, ObjectInjectToAttribute attr)&gt; list){ /*type is ClassType or MethodInfo*/ }</example>
        /// </summary>
        public ObjectInjectionReceiverAttribute(string name, string receiveMethodName)
        {
            Name = name;
            ReceiveInjectMethodName = receiveMethodName;
        }
    }


    public static class ObjectInjection
    {
        public static void InjectAll()
        {
            var receivers = new Dictionary<string, (MethodInfo, List<(object, ObjectInjectToAttribute)>)>(512);
            //var receivers = new Dictionary<string, Action<Type, ObjectInjectToAttribute>>(512);
            var injections = new List<(object, ObjectInjectToAttribute)>(512);

            foreach (var asm in TypeAssistant.AllAssemblies)
            {
                foreach (var t in asm.GetExportedTypes())
                {
                    if (t.IsGenericType)
                        continue;

                    var receiverAttr = t.GetCustomAttribute<ObjectInjectionReceiverAttribute>();
                    if (receiverAttr != null)
                    {
                        try
                        {
                            var method = t.GetMethod(receiverAttr.ReceiveInjectMethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                            //receivers.Add(receiverAttr.Name, (Action<Type, ObjectInjectToAttribute>)method.CreateDelegate(typeof(Action<Type, ObjectInjectToAttribute>)));
                            receivers.Add(receiverAttr.Name, (method, new List<(object, ObjectInjectToAttribute)>()));
                        }
                        catch (Exception ex)
                        {
                            FDebug.Error($"inject receiver error {t}.{SlimJson.SerializeToLog(receiverAttr)}\n{ex}");
                        }
                    }

                    var injectAttr = t.GetCustomAttribute<ObjectInjectToAttribute>();
                    if (injectAttr != null)
                    {
                        if (!string.IsNullOrEmpty(injectAttr.Name))
                            injections.Add((t, injectAttr));
                        foreach (var method in t.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                        {
                            injectAttr = method.GetCustomAttribute<ObjectInjectToAttribute>();
                            if (!string.IsNullOrEmpty(injectAttr?.Name))
                                injections.Add((method, injectAttr));
                        }
                    }
                }
            }


            foreach (var injection in injections)
            {
                if (!receivers.TryGetValue(injection.Item2.Name, out var receiver))
                {
                    FDebug.Error($"{injection.Item1} not found receiver: {injection.Item2.Name}");
                    continue;
                }

                receiver.Item2.Add(injection);
            }

            var args = new object[1];
            foreach (var receiver in receivers)
            {
                args[0] = receiver.Value.Item2;
                try
                {
                    receiver.Value.Item1.Invoke(null, args);
                }
                catch (Exception ex)
                {
                    FDebug.Error($"{receiver.Key}, {receiver.Value.Item1?.DeclaringType} inject error:\n{ex}");
                }
            }
        }
    }
}
