using UnityEngine;
using System.Collections.Generic;

public sealed class ConcurrentQueue<T>
{
    private Queue<T> concurrentQueue = new Queue<T>();
    private object locker = new object();

    public int Count
    {
        get
        {
            lock (locker)
            {
                return concurrentQueue.Count;
            }
        }
    }

    public void Enqueue(T item)
    {
        lock (locker)
        {
            concurrentQueue.Enqueue(item);
        }
    }

    public T Dequeue()
    {
        lock (locker)
        {
            if (concurrentQueue.Count <= 0)
                throw new System.ArgumentOutOfRangeException();
            return concurrentQueue.Dequeue();
        }
    }
}
