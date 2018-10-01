using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Cauldron.Collections
{
    /// <summary>
    /// Represents a collection of keys and values optimized for speed.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the collection.</typeparam>
    /// <typeparam name="TValue">The type of values in the collection.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    internal class FastDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEnumerable, IEnumerable<TValue>
        where TKey : class
        where TValue : class
    {
        private const int initialsize = 32;
        private int bucketLength;
        private int[] buckets;
        private FastDictionaryEntry<TKey, TValue>[] entries;
        private int nextfree;

        /// <summary>
        /// Initializes a new instance of <see cref="FastDictionary{TKey, TValue}"/>.
        /// </summary>
        public FastDictionary() => this.Initialize();

        /// <summary>
        /// Initializes a new instance of <see cref="FastDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="roughsize">The max size of the bucket.</param>
        public FastDictionary(int roughsize) => this.Initialize(this.GetRoughSize(roughsize));

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="FastDictionary{TKey, TValue}"/>.
        /// </summary>
        public int Count => nextfree;

        /// <summary>
        /// Gets a value indicating whether the <see cref="IDictionary"/> is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets an <see cref="ICollection"/> containing the keys of the <see cref="IDictionary"/>.
        /// </summary>
        public ICollection<TKey> Keys => new List<TKey>(this.GetKeys());

        /// <summary>
        /// Gets an <see cref="ICollection"/> containing the values of the <see cref="IDictionary"/>.
        /// </summary>
        public ICollection<TValue> Values => new List<TValue>(this.GetValues());

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>
        /// The value associated with the specified key.
        /// If the specified key is not found, a null is returned.
        /// </returns>
        public TValue this[TKey key]
        {
            get
            {
                int nextpos = unchecked(buckets[key.GetHashCode() & this.bucketLength]);

                if (nextpos < 0)
                    return null;

                while (true)
                {
                    var entry = unchecked(entries[nextpos]);

                    if (object.ReferenceEquals(key, entry.key) || object.Equals(key, entry.key))
                        return entry.value;

                    nextpos = entry.next;
                    if (nextpos < 0)
                        return null;
                }
            }
            set => Add(key, value, true);
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="FastDictionary{TKey, TValue}"/>.</exception>
        public void Add(TKey key, TValue value) => Add(key, value, false);

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="item">The key and value to add.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value, false);

        /// <summary>
        /// Removes all keys and values from the <see cref="FastDictionary{TKey, TValue}"/>.
        /// </summary>
        public void Clear() => this.Initialize();

        /// <summary>
        /// Determines whether the <see cref="FastDictionary{TKey, TValue}"/> contains the specified key.
        /// </summary>
        /// <param name="item">The item that contains the key.</param>
        /// <returns>
        /// true if the <see cref="FastDictionary{TKey, TValue}"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        public bool Contains(KeyValuePair<TKey, TValue> item) => this.ContainsKey(item.Key);

        /// <summary>
        /// Determines whether the <see cref="FastDictionary{TKey, TValue}"/> contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="FastDictionary{TKey, TValue}"/>.</param>
        /// <returns>
        /// true if the <see cref="FastDictionary{TKey, TValue}"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            var hash = key.GetHashCode();
            var pos = hash & this.bucketLength;
            var nextpos = this.buckets[pos];

            if (nextpos < 0)
                return false;

            while (true)
            {
                var entry = entries[nextpos];

                if (object.ReferenceEquals(key, entry.key) || object.Equals(key, entry.key))
                    return true;

                nextpos = entry.next;

                if (nextpos < 0)
                    return false;
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="FastDictionary{TKey, TValue}"/> to an <see cref="System.Array"/>, starting at a particular <see cref="System.Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="System.Array"/> that is the destination of the elements copied
        /// from <see cref="FastDictionary{TKey, TValue}"/>. The <see cref="System.Array"/> must have zero-based
        /// indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            var result = new FastDictionaryEnumerator<TKey, TValue>(this.entries).GetItems().ToArray();
            Array.Copy(result, arrayIndex, array, 0, array.Length);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new FastDictionaryEnumerator<TKey, TValue>(this.entries);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => new FastDictionaryEnumerator<TKey, TValue>(this.entries);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => new FastDictionaryEnumerator<TKey, TValue>(this.entries);

        /// <summary>
        /// Removes the value with the specified key from the <see cref="FastDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the <see cref="FastDictionary{TKey, TValue}"/>.
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
                var entry = entries[nextpos];

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
        /// Removes the value with the specified key from the <see cref="FastDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="item">The key and value to remove.</param>
        /// <returns>
        /// true if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the <see cref="FastDictionary{TKey, TValue}"/>.
        /// </returns>
        public bool Remove(KeyValuePair<TKey, TValue> item) => this.Remove(item.Key);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>true if the <see cref="FastDictionary{TKey, TValue}"/> contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            var result = this[key];
            value = result;
            return result == null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Add(TKey key, TValue value, bool overwrite)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (this.nextfree >= entries.Length)
                Resize();

            int hash = key.GetHashCode();
            int hashPos = hash & this.bucketLength;
            int entryLocation = buckets[hashPos];
            int storePos = this.nextfree;

            if (entryLocation < 0)
                this.nextfree++;
            else
            {
                int currEntryPos = entryLocation;

                while (true)
                {
                    var entry = entries[currEntryPos];

                    if (object.ReferenceEquals(key, entry.key) || object.Equals(key, entry.key))
                        if (overwrite)
                        {
                            entry.value = value;
                            return;
                        }
                        else
                            throw new ArgumentException($"An element with the same key '{key.ToString()}' already exists in the FastDictionary<TKey, TValue>.");

                    currEntryPos = entry.next;

                    if (currEntryPos < 0)
                    {
                        this.nextfree++;
                        break;
                    }
                }
            }

            buckets[hashPos] = storePos;
            entries[storePos] = new FastDictionaryEntry<TKey, TValue>
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
            for (int i = 0; i < entries.Length; i++)
                if (entries[i] != null)
                    yield return entries[i].key;
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
        private IEnumerable<TValue> GetValues()
        {
            for (int i = 0; i < entries.Length; i++)
                if (entries[i] != null)
                    yield return entries[i].value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Initialize(int roughSize = initialsize)
        {
            this.buckets = new int[roughSize];
            this.bucketLength = this.buckets.Length - 1;
            this.entries = new FastDictionaryEntry<TKey, TValue>[roughSize];

            nextfree = 0;

            for (int i = 0; i < entries.Length; i++)
                buckets[i] = -1;
        }

        private void Resize()
        {
            var newsize = this.GetRoughSize(this.buckets.Length * 2);
            var newhashes = new int[newsize];
            var newentries = new FastDictionaryEntry<TKey, TValue>[newsize];

            this.bucketLength = this.buckets.Length - 1;
            Array.Copy(entries, newentries, nextfree);

            for (int i = 0; i < newsize; i++)
                newhashes[i] = -1;

            for (int i = 0; i < nextfree; i++)
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