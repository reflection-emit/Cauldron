using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cauldron.Core.Collections
{
    /// <summary>
    /// Represents a thread-safe, unordered collection of object.
    /// </summary>
    /// <typeparam name="T">The type of the elements to be stored in the collection.</typeparam>
    public class ConcurrentCollection<T> : ICollection<T>, ICollection
    {
        private const int InitialSize = 32;
        private volatile int count;
        private ListItem[] data;
        private int lastposition = 0;
        private object syncRoot = new object();

        /// <summary>
        /// Initializes a new instance of <see cref="ConcurrentCollection{T}"/>
        /// </summary>
        public ConcurrentCollection() => this.data = new ListItem[InitialSize];

        /// <summary>
        /// Initializes a new instance of <see cref="ConcurrentCollection{T}"/>
        /// </summary>
        /// <param name="size">The initial size of the backing array.</param>
        public ConcurrentCollection(int size) => this.data = new ListItem[size];

        /// <summary>
        /// Initializes a new instance of <see cref="ConcurrentCollection{T}"/>.
        /// </summary>
        /// <param name="items">The elements to be initially added to the <see cref="ConcurrentCollection{T}"/>.</param>
        public ConcurrentCollection(IEnumerable<T> items)
        {
            var array = items.ToArray();
            this.data = new ListItem[array.Length + InitialSize];

            for (int i = 0; i < array.Length; i++)
            {
                this.data[i] = new ListItem
                {
                    item = array[i],
                };
            }

            this.count = array.Length;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ConcurrentCollection{T}"/>.
        /// </summary>
        public int Count => this.count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="ConcurrentCollection{T}"/> is read-only. This is always false.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="ConcurrentCollection{T}"/> is synchronized (thread safe). This is always true.
        /// </summary>
        public bool IsSynchronized => true;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="ConcurrentCollection{T}"/>.
        /// </summary>
        public object SyncRoot => this.syncRoot;

        /// <summary>
        /// Adds an item to the <see cref="ConcurrentCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ConcurrentCollection{T}"/>.</param>
        public void Add(T item)
        {
            lock (this.syncRoot)
            {
                var position = this.GetFreePosition();
                this.data[position] = new ListItem
                {
                    item = item,
                };

                this.count++;
            }

            this.OnItemAdded(item);
        }

        /// <summary>
        /// Adds a collection of elements to the <see cref="ConcurrentCollection{T}"/>.
        /// </summary>
        /// <param name="items">The elements to be added to the <see cref="ConcurrentCollection{T}"/>.</param>
        public void AddRange(IEnumerable<T> items)
        {
            var array = items.ToArray();

            lock (this.syncRoot)
            {
                if (array.Length > (this.data.Length - this.count))
                    this.Resize(this.data.Length + array.Length);

                for (int i = 0; i < array.Length; i++)
                {
                    var position = this.GetFreePosition();
                    this.data[position] = new ListItem
                    {
                        item = array[i],
                    };

                    this.count++;
                }
            }

            this.OnItemsAdded(array);
        }

        /// <summary>
        /// Removes all items from the <see cref="ConcurrentCollection{T}"/>.
        /// </summary>
        public void Clear()
        {
            lock (this.syncRoot)
            {
                for (int i = 0; i < this.data.Length; i++)
                    this.data[i] = null;

                this.count = 0;
            }

            this.OnClear();
        }

        /// <summary>
        /// Determines whether the <see cref="ConcurrentCollection{T}"/> contains a specific value.
        /// This is lock-free and can return true even though the element has already been removed.
        /// </summary>
        /// <param name="item">the object to locate in the <see cref="ConcurrentCollection{T}"/>.</param>
        /// <returns>true if item is found in the <see cref="ConcurrentCollection{T}"/>; otherwise, false.</returns>
        public bool Contains(T item) => Contains(item, this.data);

        /// <summary>
        /// Determines whether the <see cref="ConcurrentCollection{T}"/> contains a specific value.
        /// </summary>
        /// <param name="item">he object to locate in the <see cref="ConcurrentCollection{T}"/>.</param>
        /// <param name="lock">
        /// If true, it will be insured, that no other threads is manipulating or accessing the collection,
        /// before checking for the element.
        /// </param>
        /// <returns>true if item is found in the <see cref="ConcurrentCollection{T}"/>; otherwise, false.</returns>
        public bool Contains(T item, bool @lock)
        {
            if (!@lock)
                return Contains(item, this.data);

            lock (this.syncRoot)
            {
                return Contains(item, this.data);
            }
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ConcurrentCollection{T}"/>.
        /// The enumerator will iterate through a copy of the internal list.
        /// Changes in the collection during the iteration are ignored.
        /// </summary>
        /// <returns>A new enumerator for the contents of the <see cref="ConcurrentCollection{T}"/>.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            T[] result;

            lock (this.syncRoot)
            {
                result = this.data
                    .Where(x => x.item != null)
                    .Select(x => x.item)
                    .ToArray();
            }

            return new ConcurrentCollectionEnumerator(result);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ConcurrentCollection{T}"/>.
        /// The enumerator will iterate through a copy of the internal list.
        /// Changes in the collection during the iteration are ignored.
        /// </summary>
        /// <returns>A new enumerator for the contents of the <see cref="ConcurrentCollection{T}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            T[] result;

            lock (this.syncRoot)
            {
                result = this.data
                    .Where(x => x.item != null)
                    .Select(x => x.item)
                    .ToArray();
            }

            return new ConcurrentCollectionEnumerator(result);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ConcurrentCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ConcurrentCollection{T}"/>.</param>
        /// <returns>
        /// true if item was successfully removed from the <see cref="ConcurrentCollection{T}"/>;
        /// otherwise, false. This method also returns false if item is not found in the original
        /// <see cref="ConcurrentCollection{T}"/>.
        /// </returns>
        public bool Remove(T item)
        {
            lock (this.syncRoot)
            {
                for (int i = 0; i < this.data.Length; i++)
                    if (object.ReferenceEquals(this.data[i].item, item) || object.Equals(this.data[i].item, item))
                    {
                        this.data[i] = null;
                        this.count--;
                        this.OnRemove(item);
                        return true;
                    }
            }

            return false;
        }

        /// <summary>
        /// Removes all elements that matches the predicate from the <see cref="ConcurrentCollection{T}"/>.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        public void Remove(Func<T, bool> predicate)
        {
            var result = new List<T>();

            lock (this.syncRoot)
            {
                for (int i = 0; i < this.data.Length; i++)
                    if (this.data[i] != null && predicate(this.data[i].item))
                    {
                        result.Add(this.data[i].item);
                        this.data[i] = null;
                        this.count--;
                    }
            }

            this.OnRemove(result.ToArray());
        }

        /// <summary>
        /// Occures if all elements of the <see cref="ConcurrentCollection{T}"/> was removed.
        /// </summary>
        protected virtual void OnClear()
        {
        }

        /// <summary>
        /// Occures if a new item was added to the <see cref="ConcurrentCollection{T}"/>.
        /// </summary>
        /// <param name="item">The element that has been added to the <see cref="ConcurrentCollection{T}"/>.</param>
        protected virtual void OnItemAdded(T item)
        {
        }

        /// <summary>
        /// Occures if new items were added to the <see cref="ConcurrentCollection{T}"/>.
        /// </summary>
        /// <param name="items">The elements that were added to the <see cref="ConcurrentCollection{T}"/>.</param>
        protected virtual void OnItemsAdded(T[] items)
        {
        }

        /// <summary>
        /// Occures if an element was removed from the <see cref="ConcurrentCollection{T}"/>.
        /// </summary>
        /// <param name="item">The element that was removed.</param>
        protected virtual void OnRemove(T item)
        {
        }

        /// <summary>
        /// Occures if elements were removed from the <see cref="ConcurrentCollection{T}"/>.
        /// </summary>
        /// <param name="items">The elements that were removed.</param>
        protected virtual void OnRemove(T[] items)
        {
        }

        private static bool Contains(T item, ListItem[] items)
        {
            for (int i = 0; i < items.Length; i++)
                if (object.ReferenceEquals(items[i].item, item) || object.Equals(items[i].item, item))
                    return true;

            return false;
        }

        private int GetFreePosition()
        {
            int freePosition()
            {
                for (int i = lastposition; i < this.data.Length; i++)
                {
                    if (this.data[i] == null)
                        return i;
                }

                return -1;
            }

            var result = freePosition();

            if (result < 0 && lastposition != 0)
            {
                lastposition = 0;
                result = freePosition();
            }

            if (result < 0)
            {
                this.Resize();
                return count;
            }

            lastposition = result + 1;
            return result;
        }

        private void Resize() => Resize(this.data.Length + InitialSize);

        private void Resize(int newSize)
        {
            var newArray = new ListItem[newSize];
            Array.Copy(this.data, 0, newArray, 0, this.data.Length);
            this.data = newArray;
        }

        private class ConcurrentCollectionEnumerator : IEnumerator<T>, IEnumerator
        {
            private int currentIndex = -1;
            private T[] items;

            public ConcurrentCollectionEnumerator(T[] items) => this.items = items;

            public T Current => this.items[this.currentIndex];
            object IEnumerator.Current => this.items[this.currentIndex];

            public void Dispose() => this.items = null;

            public bool MoveNext()
            {
                while (this.currentIndex < this.items.Length && this.items[++this.currentIndex] == null)
                {
                }
                return this.currentIndex < this.items.Length;
            }

            public void Reset() => this.currentIndex = -1;
        }

        private class ListItem
        {
            public T item;
        }
    }
}