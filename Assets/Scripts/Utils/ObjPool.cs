using System;
using System.Collections.Generic;
using System.Collections;
namespace Utils
{
    public class ObjPool<T> where T : ObjPoolable, new()
    {
        List<T> used = new List<T>();
        List<T> idle = new List<T>();

        public T Create()
        {
            if (idle.Count > 0)
            {
                T o = idle[0];
                idle.RemoveAt(0);
                if(!used.Contains(o))
                    used.Add(o);
                return o;
            }
            T ret = new T();
            used.Add(ret);
            return ret;
        }

        public void GiveBack(T o)
        {
            if (used.Contains(o))
                used.Remove(o);
            if (!idle.Contains(o))
                idle.Add(o);
        }
    }
}