//==================={By Qcbf|qcbf@qq.com|7/14/2021 3:05:18 PM}===================

using FLib;
using System;
using System.Collections.Generic;

namespace FLib
{
    public static class FTime
    {
        public static DateTime CacheTimestampStart = new(1970, 1, 1, TimeZoneInfo.Local.BaseUtcOffset.Hours, 0, 0);

        public static uint TimestampUtc
        {
            get => DateToTimestamp(DateTime.UtcNow);
        }

        public static long TimestampMillisecondsUtc
        {
            get => DateToTimestampMS(DateTime.UtcNow);
        }

        public static uint Timestamp
        {
            get => DateToTimestamp(DateTime.Now);
        }

        public static long TimestampMilliseconds
        {
            get => DateToTimestampMS(DateTime.Now);
        }


        /// <summary>
        /// 时间戳转换c#时间
        /// </summary>
        public static DateTime TimestampToDate(long timestamp, bool isUtc = false)
        {
            var t = CacheTimestampStart.AddSeconds(timestamp);
            if (isUtc) t = TimeZoneInfo.ConvertTimeFromUtc(t, TimeZoneInfo.Local);
            return t;
        }
        /// <summary>
		/// 时间戳转换c#时间
		/// </summary>
		public static DateTime TimestampMSToDate(long timestamp, bool isUtc = false)
        {
            var t = CacheTimestampStart.AddMilliseconds(timestamp);
            if (isUtc) t = TimeZoneInfo.ConvertTimeFromUtc(t, TimeZoneInfo.Local);
            return t;
        }
        /// <summary>
        /// c#时间转换为时间戳
        /// </summary>
        public static uint DateToTimestamp(in DateTime date)
        {
            return (uint)(date - CacheTimestampStart).TotalSeconds;
        }
        /// <summary>
        /// c#时间转换为时间戳(毫秒)
        /// </summary>
        public static long DateToTimestampMS(in DateTime date)
        {
            return (long)(date - CacheTimestampStart).TotalMilliseconds;
        }



    }
}
