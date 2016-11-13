using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cauldron.Core.Collections
{
    /// <summary>
    /// Provides the abstract thread-safe base class for a collection whose keys are embedded in the values.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the collection.</typeparam>
    /// <typeparam name="TItem">The type of items in the collection.</typeparam>
    public abstract class ConcurrentKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem>
    {
        private object lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentKeyedCollectionEx{TKey, TItem}"/> class that uses the specified equality comparer.
        /// </summary>
        /// <param name="comparer">
        /// The implementation of the <see cref="IEqualityComparer{T}"/> generic
        /// interface to use when comparing keys, or null to use the default equality comparer
        /// for the type of the key, obtained from <see cref="EqualityComparer{T}.Default"/>.
        /// </param>
        protected ConcurrentKeyedCollection(IEqualityComparer<TKey> comparer) : base(comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentKeyedCollectionEx{TKey, TItem}"/> class that uses the specified equality comparer.
        /// </summary>
        /// <param name="comparer">
        /// The implementation of the <see cref="IEqualityComparer{T}"/> generic
        /// interface to use when comparing keys, or null to use the default equality comparer
        /// for the type of the key, obtained from <see cref="EqualityComparer{T}.Default"/>.
        /// </param>
        /// <param name="dictionaryCreationThreshold">
        /// The number of elements the collection can hold without creating a lookup dictionary
        /// (0 creates the lookup dictionary when the first item is added), or –1 to specify
        /// that a lookup dictionary is never created.
        /// </param>
        protected ConcurrentKeyedCollection(IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold) : base(comparer, dictionaryCreationThreshold)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentKeyedCollectionEx{TKey, TItem}"/> class that uses the specified equality comparer.
        /// </summary>
        protected ConcurrentKeyedCollection() : base()
        {
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ConcurrentKeyedCollection{TKey, TItem}"/>
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements should be added to the end of the <see cref="ConcurrentKeyedCollection{TKey, TItem}"/>.
        /// The collection itself cannot be null, but it can contain elements that are null,
        /// if type T is a reference type.
        /// </param>
        public void AddRange(IEnumerable<TItem> collection)
        {
            lock (this.lockObject)
            {
                foreach (var item in collection)
                    base.InsertItem(this.Count, item);
            }
        }

        /// <summary>
        /// Removes all elements from the <see cref="ConcurrentKeyedCollection{TKey, TItem}"/>
        /// </summary>
        protected override void ClearItems()
        {
            lock (this.lockObject)
            {
                base.ClearItems();
            }
        }

        /// <summary>
        /// Inserts an element into the <see cref="ConcurrentKeyedCollection{TKey, TItem}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, TItem item)
        {
            lock (this.lockObject)
            {
                base.InsertItem(index, item);
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="ConcurrentKeyedCollection{TKey, TItem}"/>
        /// </summary>
        /// <param name="index">The index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            lock (this.lockObject)
            {
                base.RemoveItem(index);
            }
        }

        /// <summary>
        /// Replaces the item at the specified index with the specified item.
        /// </summary>
        /// <param name="index">The zero-based index of the item to be replaced.</param>
        /// <param name="item">The new item.</param>
        protected override void SetItem(int index, TItem item)
        {
            lock (this.lockObject)
            {
                base.SetItem(index, item);
            }
        }
    }
}