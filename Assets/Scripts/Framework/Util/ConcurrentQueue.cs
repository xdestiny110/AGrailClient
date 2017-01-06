using System;
using System.Collections.Generic;

namespace Framework
{
    public sealed class ConcurrentQueue<T>
    {
        private readonly Queue<T> inner = new Queue<T>();
        private readonly object locker = new object();

        public int Count
        {
            get { lock(locker) { return inner.Count; } }
        }

        public void Enqueue(T item)
        {
            lock (locker)
            {
                inner.Enqueue(item);
            }
        }

        public T Dequeue()
        {
            lock (locker)
            {
                if (inner.Count <= 0)
                    throw new ArgumentOutOfRangeException();
                return inner.Dequeue();
            }            
        }

        public void clear()
        {
            lock (locker)
            {
                inner.Clear();
            }
        }

    }
}


