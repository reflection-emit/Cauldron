using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Core.Collections
{
    /// <summary>
    /// Represents a thread-safe list of items that can be accessed by multiple threads concurrently.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public class ConcurrentList<T> : IEnumerable<T>, IList<T>, ICollection<T>, IEnumerable, IList, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>
    {
        private readonly List<T> internalList = new List<T>();
        private readonly object objectLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentList{T}"/> class that is empty and has the default initial capacity.
        /// </summary>
        public ConcurrentList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentList{T}"/> class that
        /// contains elements copied from the specified collection and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <exception cref="ArgumentNullException">collection is null.</exception>
        public ConcurrentList(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            this.AddRange(collection);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ConcurrentList{T}"/>
        /// </summary>
        public int Count
        {
            get
            {
                lock (this.objectLock)
                {
                    return this.internalList.Count;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ConcurrentList{T}"/> has a fixed size.
        /// </summary>
        public bool IsFixedSize { get { return (this.internalList as IList).IsFixedSize; } }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ConcurrentList{T}"/> is read-only.
        /// </summary>
        public bool IsReadOnly { get { return (this.internalList as IList).IsReadOnly; } }

        /// <summary>
        ///
        /// </summary>
        public bool IsSynchronized { get { return (this.internalList as IList).IsSynchronized; } }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="ConcurrentList{T}"/> is synchronized (thread safe).
        /// </summary>
        public object SyncRoot { get { return this.objectLock; } }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">index is less than 0.-or-index is equal to or greater than <see cref="ConcurrentList{T}.Count"/>.</exception>
        object IList.this[int index]
        {
            get
            {
                lock (this.objectLock)
                {
                    return this.internalList[index];
                }
            }
            set
            {
                lock (this.objectLock)
                {
                    this.internalList[index] = (T)value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">index is less than 0.-or-index is equal to or greater than <see cref="ConcurrentList{T}.Count"/>.</exception>
        T IList<T>.this[int index]
        {
            get
            {
                lock (this.objectLock)
                {
                    return this.internalList[index];
                }
            }
            set
            {
                lock (this.objectLock)
                {
                    this.internalList[index] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">index is less than 0.-or-index is equal to or greater than <see cref="ConcurrentList{T}.Count"/>.</exception>
        public T this[int index]
        {
            get
            {
                lock (this.objectLock)
                {
                    return this.internalList[index];
                }
            }
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="ConcurrentList{T}"/>
        /// </summary>
        /// <param name="item">
        /// The object to be added to the end of the <see cref="ConcurrentList{T}"/>
        /// The value can be null for reference types.
        /// </param>
        public void Add(T item)
        {
            lock (this.objectLock)
            {
                this.internalList.Add(item);
            }
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ConcurrentList{T}"/>
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements should be added to the end of the <see cref="ConcurrentList{T}"/>.
        /// The collection itself cannot be null, but it can contain elements that are null,
        /// if type T is a reference type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// collection is null.
        /// </exception>
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            lock (this.objectLock)
            {
                this.internalList.AddRange(collection);
            }
        }

        /// <summary>
        /// Removes all elements from the <see cref="ConcurrentList{T}"/>
        /// </summary>
        public void Clear()
        {
            lock (this.objectLock)
            {
                this.internalList.Clear();
            }
        }

        /// <summary>
        /// Determines whether an element is in the <see cref="ConcurrentList{T}"/>
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ConcurrentList{T}"/>. The value can be null for reference types.</param>
        /// <returns>true if item is found in the <see cref="ConcurrentList{T}"/>; otherwise, false.</returns>
        public bool Contains(T item)
        {
            return this.Clone().Contains(item);
        }

        /// <summary>
        /// Determines whether an element is in the <see cref="ConcurrentList{T}"/>
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>true if item is found in the <see cref="ConcurrentList{T}"/>; otherwise, false.</returns>
        public bool Contains(Func<T, bool> predicate)
        {
            lock (this.objectLock)
            {
                for (int i = 0; i < this.internalList.Count; i++)
                {
                    if (predicate(this.internalList[i]))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether an element is in the <see cref="ConcurrentList{T}"/>
        /// </summary>
        /// <param name="value">The object to locate in the <see cref="ConcurrentList{T}"/>. The value can be null for reference types.</param>
        /// <returns>true if item is found in the <see cref="ConcurrentList{T}"/>; otherwise, false.</returns>
        public bool Contains(object value)
        {
            return this.Clone().Contains((T)value);
        }

        /// <summary>
        /// Copies a range of elements from the <see cref="ConcurrentList{T}"/> to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from <see cref="ConcurrentList{T}"/>. The System.Array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException">array is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <see cref="ConcurrentList{T}"/> is greater
        /// than the available space from arrayIndex to the end of the destination array.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.Clone().CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies a range of elements from the <see cref="ConcurrentList{T}"/> to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from <see cref="ConcurrentList{T}"/>. The System.Array must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException">array is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <see cref="ConcurrentList{T}"/> is greater
        /// than the available space from arrayIndex to the end of the destination array.
        /// </exception>
        public void CopyTo(Array array, int index)
        {
            (this.Clone() as IList).CopyTo(array, index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ConcurrentList{T}"/>
        /// </summary>
        /// <returns>A <see cref="List{T}.Enumerator"/> for the <see cref="ConcurrentList{T}"/>.</returns>
        public IEnumerator<T> GetEnumerator() =>
            this.Clone().GetEnumerator();

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ConcurrentList{T}"/>
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ConcurrentList{T}"/>. The value can be null for reference types.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the <see cref="ConcurrentList{T}"/></returns>
        bool ICollection<T>.Remove(T item)
        {
            lock (this.objectLock)
            {
                return this.internalList.Remove(item);
            }
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="ConcurrentList{T}"/>
        /// </summary>
        /// <param name="value">
        /// The object to be added to the end of the <see cref="ConcurrentList{T}"/>
        /// The value can be null for reference types.
        /// </param>
        int IList.Add(object value)
        {
            lock (this.objectLock)
            {
                return (this.internalList as IList).Add((T)value);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ConcurrentList{T}"/>
        /// </summary>
        /// <param name="value">The object to remove from the <see cref="ConcurrentList{T}"/>. The value can be null for reference types.</param>
        void IList.Remove(object value)
        {
            lock (this.objectLock)
            {
                this.internalList.Remove((T)value);
            }
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first
        /// occurrence within the entire <see cref="ConcurrentList{T}"/>
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ConcurrentList{T}"/>. The value can be null for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire <see cref="ConcurrentList{T}"/>, if found; otherwise, –1.</returns>
        public int IndexOf(T item)
        {
            return this.Clone().IndexOf(item);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first
        /// occurrence within the entire <see cref="ConcurrentList{T}"/>
        /// </summary>
        /// <param name="value">The object to locate in the <see cref="ConcurrentList{T}"/>. The value can be null for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire <see cref="ConcurrentList{T}"/>, if found; otherwise, –1.</returns>
        public int IndexOf(object value)
        {
            return this.Clone().IndexOf((T)value);
        }

        /// <summary>
        /// Inserts an element into the <see cref="ConcurrentList{T}"/> at the specified index
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="ArgumentOutOfRangeException">index is less than 0.-or-index is greater than <see cref="ConcurrentList{T}.Count"/>.</exception>
        public void Insert(int index, T item)
        {
            lock (this.objectLock)
            {
                this.internalList.Insert(index, item);
            }
        }

        /// <summary>
        /// Inserts an element into the <see cref="ConcurrentList{T}"/> at the specified index
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="value">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="ArgumentOutOfRangeException">index is less than 0.-or-index is greater than <see cref="ConcurrentList{T}.Count"/>.</exception>
        public void Insert(int index, object value)
        {
            lock (this.objectLock)
            {
                this.internalList.Insert(index, (T)value);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ConcurrentList{T}"/>
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ConcurrentList{T}"/>. The value can be null for reference types.</param>
        public void Remove(T item)
        {
            lock (this.objectLock)
            {
                this.internalList.Remove(item);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ConcurrentList{T}"/>
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A list of items that was removed from the collection</returns>
        public IEnumerable<T> Remove(Func<T, bool> predicate)
        {
            lock (this.objectLock)
            {
                var result = new List<T>();

                for (int i = 0; i < this.internalList.Count; i++)
                {
                    var item = this.internalList[i];

                    if (predicate(item) && this.internalList.Remove(item))
                    {
                        this.internalList.Remove(item);
                        result.Add(item);
                        i--;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements to remove.</param>
        /// <exception cref="ArgumentNullException">match is null.</exception>
        public void RemoveAll(Predicate<T> match)
        {
            lock (this.objectLock)
            {
                this.internalList.RemoveAll(match);
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="ConcurrentList{T}"/>
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than 0.-or-index is equal to or greater than <see cref="ConcurrentList{T}.Count"/>.
        /// </exception>
        public void RemoveAt(int index)
        {
            lock (this.objectLock)
            {
                this.internalList.RemoveAt(index);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ConcurrentList{T}"/>
        /// </summary>
        /// <returns>A <see cref="List{T}.Enumerator"/> for the <see cref="ConcurrentList{T}"/>.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Clone().GetEnumerator();
        }

        /// <summary>
        /// Copies the elements of the <see cref="ConcurrentList{T}"/> to a new array.
        /// </summary>
        /// <returns>An array containing copies of the elements of the <see cref="ConcurrentList{T}"/></returns>
        public T[] ToArray()
        {
            lock (this.objectLock)
            {
                return this.internalList.AsParallel().ToArray();
            }
        }

        /// <summary>
        /// Creates a <see cref="List{T}"/> from an <see cref="ConcurrentList{T}"/>.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> that contains elements from the input sequence.</returns>
        public List<T> ToList()
        {
            return this.Clone();
        }

        private List<T> Clone()
        {
            lock (this.objectLock)
            {
                return new List<T>(this.internalList.AsParallel());
            }
        }
    }
}