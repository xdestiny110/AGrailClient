using System;
using System.Collections;
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

    public sealed class ConcurrentList<T> : IEnumerable<T>
    {
        private readonly List<T> inner = new List<T>();
        private readonly object locker = new object();

        public int Count
        {
            get { lock (locker) { return inner.Count; } }
        }

        public void Add(T item)
        {
            lock (locker)
            {
                inner.Add(item);
            }
        }

        public void Remove(T item)
        {
            lock (locker)
            {
                inner.Remove(item);
            }
        }

        public T this[int idx]
        {
            get
            {
                lock (locker)
                {
                    return inner[idx];
                }
            }
            set
            {
                lock (locker)
                {
                    inner[idx] = value;
                }
            }
        }

        public void clear()
        {
            lock (locker)
            {
                inner.Clear();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (locker)
            {
                foreach (var v in inner)
                    yield return v;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (locker)
            {
                foreach (var v in inner)
                    yield return v;
            }
        }
    }

}


