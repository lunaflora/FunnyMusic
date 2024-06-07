//==================={By Qcbf|qcbf@qq.com|12/5/2023 5:37:16 PM}===================

using FLib;
using System;
using System.Collections.Generic;
using System.Threading;

namespace FLib
{
    /// <summary>
    /// 
    /// </summary>
    public class SmartSpinLocker
    {
        public SpinLock Locker;
        public bool IsLocked;
        
        public readonly struct Unlocker : IDisposable
        {
            public readonly SmartSpinLocker Locker;
            public readonly bool IsEmpty => Locker == null;
            public Unlocker(SmartSpinLocker locker) => Locker = locker;
            public readonly void Dispose() => Locker?.Unlock();
        }

        /// <summary>
        /// 
        /// </summary>
        public Unlocker Lock()
        {
            var isLocked = false;
            if (!Locker.IsHeldByCurrentThread)
            {
                Locker.Enter(ref isLocked);
                IsLocked = isLocked;
                if (IsLocked)
                    return new Unlocker(this);
            }
            return default;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Unlock(bool useMemoryBarrier = false)
        {
            if (IsLocked)
            {
                IsLocked = false;
                Locker.Exit(useMemoryBarrier);
            }
        }

    }
}
