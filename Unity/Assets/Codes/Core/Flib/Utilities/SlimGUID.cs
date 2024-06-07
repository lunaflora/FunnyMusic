//==================={By Qcbf|qcbf@qq.com|7/10/2022 1:01:02 PM}===================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using FLib;

namespace FLib
{
    public struct SlimGUID
    {
        private static int Step;
        public long Raw;

        /// <summary>
        ///
        /// </summary>
        public int GetDeviceId(byte deviceBitSize)
        {
            return (int)((ulong)Raw >> (64 - deviceBitSize));
        }

        /// <summary>
        ///
        /// </summary>
        public long GetMilliseconds()
        {
            return ConvertMilliseconds(Raw);
        }

        /// <summary>
        ///
        /// </summary>
        public static SlimGUID Create(int deviceId, byte deviceBitSize)
        {
            const int maxBit = 64;
            var deviceBitOffset = maxBit - deviceBitSize;
            var stepBitMask = (1 << maxBit - deviceBitSize - 42) - 1;
            return new() { Raw = ((long)deviceId << deviceBitOffset) | ((long)(Interlocked.Increment(ref Step) & stepBitMask) << 42) | ConvertMilliseconds(FTime.TimestampMillisecondsUtc) };
        }

        /// <summary>
        ///
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ConvertMilliseconds(long ms)
        {
            return ms & ((1L << 42) - 1);
        }

        /// <summary>
        /// per seconds generate 63000 unique ids
        /// </summary>
        public static SlimGUID Create(ushort deviceId) => Create(deviceId, 16);

        /// <summary>
        /// per seconds generate 16383000 unique ids
        /// </summary>
        public static SlimGUID Create(byte deviceId) => Create(deviceId, 8);

        /// <summary>
        /// per seconds generate 4194303000 unique ids
        /// </summary>
        public static SlimGUID Create() => Create(0, 0);


        public override string ToString() => Raw.ToString();
        public static implicit operator long(SlimGUID v) => v.Raw;
    }
}
