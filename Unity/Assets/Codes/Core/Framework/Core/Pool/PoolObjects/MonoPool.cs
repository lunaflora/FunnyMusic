﻿using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 搬运
    /// 用于创建一些集合类型
    /// </summary>
    public class MonoPool: IDisposable
    {
        private readonly Dictionary<Type, Queue<object>> pool = new Dictionary<Type, Queue<object>>();
        
        public static MonoPool Instance = new MonoPool();
        
        private MonoPool()
        {
        }

        public object Fetch(Type type)
        {
            Queue<object> queue = null;
            if (!pool.TryGetValue(type, out queue))
            {
                return Activator.CreateInstance(type);
            }

            if (queue.Count == 0)
            {
                return Activator.CreateInstance(type);
            }
            return queue.Dequeue();
        }

        public void Recycle(object obj)
        {
            Type type = obj.GetType();
            Queue<object> queue = null;
            if (!pool.TryGetValue(type, out queue))
            {
                queue = new Queue<object>();
                pool.Add(type, queue);
            }
            queue.Enqueue(obj);
        }

        public void Dispose()
        {
            this.pool.Clear();
        }
    }
    
    public static class PoolHelper
    {
        public static T Fetch<T>(this MonoPool monoPool) where T : class
        {
            return monoPool.Fetch(typeof(T)) as T;
        }
        
    }
}