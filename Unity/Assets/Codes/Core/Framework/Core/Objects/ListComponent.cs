using System;
using System.Collections.Generic;

namespace Framework
{
    public class ListComponent<T>: List<T>, System.IDisposable
    {
        public static ListComponent<T> Create()
        {
            return MonoPool.Instance.Fetch(typeof (ListComponent<T>)) as ListComponent<T>;
        }

        public void Dispose()
        {
            this.Clear();
            MonoPool.Instance.Recycle(this);
        }
    }
}