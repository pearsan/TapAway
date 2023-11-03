using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Falcon.FalconCore.Scripts
{
    public class LimitQueue<T>
    {
        private readonly ConcurrentQueue<T> concurrentQueue = new ConcurrentQueue<T>();

        private readonly int maxSize;

        public int Count
        {
            get { return concurrentQueue.Count; }
        }

        public bool IsEmpty
        {
            get { return concurrentQueue.IsEmpty; }
        }

        public LimitQueue(int maxSize)
        {
            this.maxSize = maxSize;
        }

        public void Enqueue(T item)
        {
            if (concurrentQueue.Count > maxSize)
            {
                T ignored;
                concurrentQueue.TryDequeue(out ignored);
            }
            
            concurrentQueue.Enqueue(item);
        }
        
        public void EnqueueAll(IEnumerable<T> items)
        {
            if (concurrentQueue.Count > maxSize)
            {
                T ignored;
                concurrentQueue.TryDequeue(out ignored);
            }

            foreach (T item in items)
            {
                concurrentQueue.Enqueue(item);

            }
        }

        public T Dequeue()
        {
            int tryFail = 0;
            while (tryFail < 3)
            {
                T item;
                if (concurrentQueue.TryDequeue(out item))
                {
                    return item;
                }
                else
                {
                    tryFail++;
                }
            }
            return default(T);
        }
        
        public List<T> DequeueAll()
        {
            List<T> result = new List<T>();
            int tryFail = 0;
            while (tryFail < 3 && !concurrentQueue.IsEmpty)
            {
                T item;
                if (concurrentQueue.TryDequeue(out item))
                {
                    result.Add(item);
                }
                else
                {
                    tryFail++;
                }
            }
            return result;
        }

        public List<T> ToListInstance()
        {
            return new List<T>(concurrentQueue);
        }

        public void Clear()
        {
            T ignored;
            while (concurrentQueue.TryDequeue(out ignored))
            {
            }
        }

        public void Remove(T item)
        {
            List<T> list = ToListInstance();
            Clear();
            list.Remove(item);
            EnqueueAll(list);
        }

        public bool Contain(T item)
        {
            return concurrentQueue.Contains(item);
        }
    }
}