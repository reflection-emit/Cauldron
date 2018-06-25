using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Core.Collections
{
    public class ConcurrentList<T> : IList<T>, IList
        where T : class
    {
        private const int InitialSize = 32;
        private volatile int count;
        private T[] data;
        private object syncRoot;

        public ConcurrentList()
        {
            this.data = new T[InitialSize];
        }

        public ConcurrentList(int size)
        {
            this.data = new T[size];
        }

        public int Count => this.count;
        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public bool IsSynchronized => true;

        public object SyncRoot => this.syncRoot;

        public T this[int index]
        {
            get
            {
            }
        }

        object IList.this[int index] { get; set; }

        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            lock (this.syncRoot)
            {
                var position = this.GetFreePosition();
                this.data[position] = item;

                this.count++;
            }
        }

        public int Add(object value)
        {
            var item = (T)value;

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            lock (this.syncRoot)
            {
                var position = this.GetFreePosition();
                this.data[position] = item;

                this.count++;
                return position;
            }
        }

        public void Clear()
        {
            lock (this.syncRoot)
            {
                for (int i = 0; i < this.data.Length; i++)
                    this.data[i] = null;

                this.count = 0;
            }
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        private int GetFreePosition()
        {
            for (int i = 0; i < this.data.Length; i++)
            {
                if (this.data[i] == null)
                    return i;
            }

            this.Resize();
            return count;
        }

        private void Resize()
        {
            var newArray = new T[this.data.Length + InitialSize];
            Array.Copy(this.data, 0, newArray, 0, this.data.Length);
            this.data = newArray;
        }
    }
}