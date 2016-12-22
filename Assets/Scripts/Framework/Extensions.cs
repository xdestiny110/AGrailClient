﻿using System.Collections.Generic;

public static class Extensions{

    #region Dictionary扩展
    public static bool TryAdd<U,V>(this Dictionary<U,V> dict, U key, V value)
    {
        if (dict.ContainsKey(key))
            return false;
        else
        {
            dict.Add(key, value);
            return true;
        }
    }

    public static void AddOrReplace<U,V>(this Dictionary<U,V> dict, U key, V value)
    {
        if (dict.ContainsKey(key))
            dict[key] = value;
        else
            dict.Add(key, value);
    }
    #endregion

    #region stack扩展
    public static bool TryPeek<T>(this Stack<T> stack, ref T item)
    {
        if (stack.Count > 0)
        {
            item = stack.Peek();
            return true;
        }
        return false;
    }

    public static bool TryPop<T>(this Stack<T> stack, ref T item)
    {
        if(stack.Count > 0)
        {
            item = stack.Pop();
            return true;
        }
        return false;
    }
    #endregion

}
