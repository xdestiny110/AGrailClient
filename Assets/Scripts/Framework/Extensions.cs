using System.Collections.Generic;

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

}
