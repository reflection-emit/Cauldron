using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Cauldron.Collections
{
    /// <summary>
    /// Represents a collection whose keys are embedded in the values that is optimized for speed.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the collection.</typeparam>
    /// <typeparam name="TItem">The type of items in the collection.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public class FastKeyedCollection<TKey, TItem> : IEnumerable, IEnumerable<TItem>, ICollection<TItem> where TItem : class
    {
        private const int initialsize = 32;
        private int bucketLength;
        private int[] buckets;
        private FastDictionaryEntry<TKey, TItem>[] entries;
        private Func<TItem, TKey> keySelector;
        private int nextfree;

        /// <summary>
        /// Initializes a new instance of <see cref="FastKeyedCollection{TKey, TValue}"/>.
        /// </summary>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        public FastKeyedCollection(Func<TItem, TKey> keySelector)
        {
            this.keySelector = keySelector;
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FastKeyedCollection{TKey, TValue}"/>.
        /// </summary>
        /// <param name="items">A collection of elements to initially add to the <see cref="FastKeyedCollection{TKey, TItem}"/></param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        public FastKeyedCollection(IEnumerable<TItem> items, Func<TItem, TKey> keySelector)
        {
            var source = items.ToArray();
            this.keySelector = keySelector;
            this.Initialize(source.Length);

            for (int i = 0; i < source.Length; i++)
                this.Add(source[i]);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FastKeyedCollection{TKey, TValue}"/>.
        /// </summary>
        /// <param name="roughsize">The max size of the bucket.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        public FastKeyedCollection(int roughsize, Func<TItem, TKey> keySelector)
        {
            this.keySelector = keySelector;
            this.Initialize(this.GetRoughSize(roughsize));
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="FastKeyedCollection{TKey, TValue}"/>.
        /// </summary>
        public int Count => this.nextfree;

        /// <summary>
        /// Gets a value indicating whether the <see cref="ICollection"/> is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets an <see cref="ICollection"/> containing the keys of the <see cref="FastKeyedCollection{TKey, TValue}"/>.
        /// </summary>
        public ICollection<TKey> Keys => new List<TKey>(this.GetKeys());

        /// <summary>
        /// Gets an <see cref="ICollection"/> containing the values of the <see cref="FastKeyedCollection{TKey, TValue}"/>.
        /// </summary>
        public ICollection<TItem> Values => new List<TItem>(this.GetValues());

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>
        /// The value associated with the specified key.
        /// If the specified key is not found, a null is returned.
        /// </returns>
        public TItem this[TKey key]
        {
            get
            {
                int nextpos = unchecked(this.buckets[key.GetHashCode() & this.bucketLength]);

                if (nextpos < 0)
                    return null;

                while (true)
                {
                    var entry = unchecked(this.entries[nextpos]);

                    if (object.ReferenceEquals(key, entry.key) || object.Equals(key, entry.key))
                        return entry.value;

                    nextpos = entry.next;
                    if (nextpos < 0)
                        return null;
                }
            }
            set => this.Add(key, value, true);
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="item">The value of the element to add. The value can be null for reference types.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="FastKeyedCollection{TKey, TValue}"/>.</exception>
        public void Add(TItem item) => this.Add(this.keySelector(item), item, false);

        /// <summary>
        /// Removes all keys and values from the <see cref="FastKeyedCollection{TKey, TValue}"/>.
        /// </summary>
        public void Clear() => this.Initialize();

        /// <summary>
        /// Determines whether the <see cref="FastKeyedCollection{TKey, TValue}"/> contains the specified item described by the key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="FastKeyedCollection{TKey, TValue}"/>.</param>
        /// <returns>
        /// true if the <see cref="FastKeyedCollection{TKey, TValue}"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        public bool Contains(TKey key)
        {
            var hash = key.GetHashCode();
            var pos = hash & this.bucketLength;
            var nextpos = this.buckets[pos];

            if (nextpos < 0)
                return false;

            while (true)
            {
                var entry = this.entries[nextpos];

                if (object.ReferenceEquals(key, entry.key) || object.Equals(key, entry.key))
                    return true;

                nextpos = entry.next;

                if (nextpos < 0)
                    return false;
            }
        }

        /// <summary>
        /// Determines whether the <see cref="FastKeyedCollection{TKey, TValue}"/> contains the specified item.
        /// </summary>
        /// <param name="item">The item to locate in the <see cref="FastKeyedCollection{TKey, TValue}"/>.</param>
        /// <returns>
        /// true if the <see cref="FastKeyedCollection{TKey, TValue}"/> contains the element; otherwise, false.
        /// </returns>
        public bool Contains(TItem item) => this.GetValues().Any(x => object.ReferenceEquals(item, x) || object.Equals(item, x));

        /// <summary>
        /// Copies the elements of the <see cref="FastKeyedCollection{TKey, TValue}"/> to an <see cref="System.Array"/>, starting at a particular <see cref="System.Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="System.Array"/> that is the destination of the elements copied
        /// from <see cref="FastKeyedCollection{TKey, TValue}"/>. The <see cref="System.Array"/> must have zero-based
        /// indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(TItem[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            var result = new FastDictionaryEnumerator<TKey, TItem>(this.entries).GetItems().ToArray();
            Array.Copy(result, arrayIndex, array, 0, array.Length);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => new FastDictionaryEnumerator<TKey, TItem>(this.entries);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator() => new FastDictionaryEnumerator<TKey, TItem>(this.entries);

        /// <summary>
        /// Removes the value with the specified key from the <see cref="FastKeyedCollection{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the <see cref="FastKeyedCollection{TKey, TValue}"/>.
        /// </returns>
        public bool Remove(TKey key)
        {
            var hash = key.GetHashCode();
            var pos = hash & this.bucketLength;
            var nextpos = this.buckets[pos];

            if (nextpos < 0)
                return false;

            while (true)
            {
                var entry = this.entries[nextpos];

                if (object.ReferenceEquals(key, entry.key) || object.Equals(key, entry.key))
                {
                    this.nextfree--;
                    this.buckets[pos] = -1;
                    this.entries[nextpos] = null;
                    return true;
                }

                nextpos = entry.next;

                if (nextpos < 0)
                    return false;
            }
        }

        /// <summary>
        /// Removes the element from the <see cref="FastKeyedCollection{TKey, TValue}"/>.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        /// <returns>
        /// true if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the <see cref="FastKeyedCollection{TKey, TValue}"/>.
        /// </returns>
        public bool Remove(TItem item) => this.Remove(this.keySelector(item));

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>true if the <see cref="FastKeyedCollection{TKey, TValue}"/> contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue(TKey key, out TItem value)
        {
            var result = this[key];
            value = result;
            return result != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Add(TKey key, TItem value, bool overwrite)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (this.nextfree >= this.entries.Length)
                this.Resize();

            int hash = key.GetHashCode();
            int hashPos = hash & this.bucketLength;
            int entryLocation = this.buckets[hashPos];
            int storePos = this.nextfree;

            if (entryLocation < 0)
                this.nextfree++;
            else
            {
                int currEntryPos = entryLocation;

                while (true)
                {
                    var entry = this.entries[currEntryPos];

                    if (object.ReferenceEquals(key, entry.key) || object.Equals(key, entry.key))
                        if (overwrite)
                        {
                            entry.value = value;
                            return;
                        }
                        else
                            throw new ArgumentException($"An element with the same key '{key.ToString()}' already exists in the FastKeyedCollection<TKey, TValue>.");

                    currEntryPos = entry.next;

                    if (currEntryPos < 0)
                    {
                        this.nextfree++;
                        break;
                    }
                }
            }

            this.buckets[hashPos] = storePos;
            this.entries[storePos] = new FastDictionaryEntry<TKey, TItem>
            {
                next = entryLocation,
                key = key,
                value = value,
                hashcode = (uint)hash
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<TKey> GetKeys()
        {
            for (int i = 0; i < this.entries.Length; i++)
                if (this.entries[i] != null)
                    yield return this.entries[i].key;
        }

        private int GetRoughSize(int roughSize)
        {
            int result = initialsize;

            while (true)
            {
                if (result >= roughSize)
                    return result;

                result *= 2;

                if (result > int.MaxValue)
                    break;
            }

            throw new IndexOutOfRangeException("The hash array is too big.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<TItem> GetValues()
        {
            for (int i = 0; i < this.entries.Length; i++)
                if (this.entries[i] != null)
                    yield return this.entries[i].value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Initialize(int roughSize = initialsize)
        {
            this.buckets = new int[roughSize];
            this.bucketLength = this.buckets.Length - 1;
            this.entries = new FastDictionaryEntry<TKey, TItem>[roughSize];

            this.nextfree = 0;

            for (int i = 0; i < this.entries.Length; i++)
                this.buckets[i] = -1;
        }

        private void Resize()
        {
            var newsize = this.GetRoughSize(this.buckets.Length * 2);
            var newhashes = new int[newsize];
            var newentries = new FastDictionaryEntry<TKey, TItem>[newsize];

            this.bucketLength = this.buckets.Length - 1;
            Array.Copy(this.entries, newentries, this.nextfree);

            for (int i = 0; i < newsize; i++)
                newhashes[i] = -1;

            for (int i = 0; i < this.nextfree; i++)
            {
                var pos = newentries[i].hashcode & this.bucketLength;
                var prevpos = newhashes[pos];

                newhashes[pos] = i;

                if (prevpos != -1)
                    newentries[i].next = prevpos;
            }

            this.buckets = newhashes;
            this.entries = newentries;
        }
    }
}