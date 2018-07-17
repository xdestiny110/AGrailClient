using System;
using System.Collections.Generic;

namespace Framework
{
    public static class Extensions
    {
        #region Dictionary扩展
        public static bool TryAdd<U, V>(this Dictionary<U, V> dict, U key, V value)
        {
            if (dict.ContainsKey(key))
                return false;
            else
            {
                dict.Add(key, value);
                return true;
            }
        }

        public static void AddOrReplace<U, V>(this Dictionary<U, V> dict, U key, V value)
        {
            if (dict.ContainsKey(key))
                dict[key] = value;
            else
                dict.Add(key, value);
        }
        #endregion

        #region stack扩展
        public static bool TryPeek<T>(this Stack<T> stack, out T item)
        {
            if (stack.Count > 0)
            {
                item = stack.Peek();
                return true;
            }
            item = default(T);
            return false;
        }

        public static bool TryPop<T>(this Stack<T> stack, out T item)
        {
            if (stack.Count > 0)
            {
                item = stack.Pop();
                return true;
            }
            item = default(T);
            return false;
        }
        #endregion

        #region DateTime扩展
        public static DateTime dt1970 = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        public static long GetMiliSecFrom1970(this DateTime dt)
        {
            return (long)dt.Subtract(dt1970).TotalMilliseconds;
        }

        public static DateTime GetDateTime(this long miliSec)
        {
            TimeSpan toNow = new TimeSpan(miliSec * 10000);
            return dt1970.Add(toNow);
        }
        public static DateTime GetDiffTime(this long miliSec)
        {
            TimeSpan toNow = new TimeSpan(miliSec * 10000);
            return new DateTime().Add(toNow);
        }
        #endregion
    }
}

