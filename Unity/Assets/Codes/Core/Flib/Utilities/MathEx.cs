﻿//==================={By Qcbf|qcbf@qq.com|5/28/2021 5:33:39 PM}===================

// #undef FNUM

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FLib
{
    public class MathEx
    {
#if FIXED_POINT
        public static readonly FNum Eplison = FNum.Epsilon;
        public static readonly FNum Pi = FNum.PI;
        public static readonly FNum Deg2Rad = Pi / 180;
        public static readonly FNum Rad2Deg = 1 / Deg2Rad;
#else
        public const float Eplison = 1e-6f;
        public const float Pi = 3.14159274f;
        public const float Deg2Rad = Pi / 180;
        public const float Rad2Deg = 1 / Deg2Rad;
#endif
        public static int GetNextPowerOfTwo(int v) => (int)GetNextPowerOfTwo((long)v);

        /// <summary>
        /// 如果当前不是2的次幂寻找下一个2的次幂的数字
        /// </summary>
        public static long GetNextPowerOfTwo(long v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;
            return v;
        }

        /// <summary>
        /// 
        /// </summary>
        public static long Clamp(long value, long min, long max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
#if FIXED_POINT
        public static FNum Clamp01(FNum value)
#else
        public static float Clamp01(float value)
#endif
        {
            if (value < 0)
                return 0;
            return value > 1 ? 1 : value;
        }

#if FIXED_POINT
        public static FNum Clamp(FNum value, FNum min, FNum max)
#else
        public static float Clamp(float value, float min, float max)
#endif
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }

            return value;
        }
    }
}
