﻿using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class EntityPool: IDisposable
    {
        private readonly Dictionary<Type, Queue<Entity>> pool = new Dictionary<Type, Queue<Entity>>();
        
        public static EntityPool Instance = new EntityPool();
        
        private EntityPool()
        {
        }

        public Entity Fetch(Type type)
        {
            Queue<Entity> queue = null;
            if (!pool.TryGetValue(type, out queue))
            {
                return Activator.CreateInstance(type) as Entity;
            }

            if (queue.Count == 0)
            {
                return Activator.CreateInstance(type) as Entity;
            }
            return queue.Dequeue();
        }

        public void Recycle(Entity obj)
        {
            Type type = obj.GetType();
            Queue<Entity> queue = null;
            if (!pool.TryGetValue(type, out queue))
            {
                queue = new Queue<Entity>();
                pool.Add(type, queue);
            }
            queue.Enqueue(obj);
        }

        public void Dispose()
        {
            this.pool.Clear();
        }
    }
}