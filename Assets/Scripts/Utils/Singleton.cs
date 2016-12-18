using System.Collections;
using System.Collections.Generic;

public abstract class Singleton<T> where T : new()
{
    private static T instance;

    public static T GetInstance()
    {
        if (null == instance)
            instance = new T();
        return instance;
    }
}