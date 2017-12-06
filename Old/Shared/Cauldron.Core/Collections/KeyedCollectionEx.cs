using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cauldron.Core.Collections
{
    /// <summary>
    /// Provides an implementation of the <see cref="KeyedCollection{TKey, TItem}"/> that can define the key on construction
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the collection.</typeparam>
    /// <typeparam name="TItem">The type of items in the collection.</typeparam>
    public class KeyedCollectionEx<TKey, TItem> : KeyedCollection<TKey, TItem>
    {
        private Func<TItem, TKey> keySelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedCollectionEx{TKey, TItem}"/> class that uses the default equality comparer.
        /// </summary>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        public KeyedCollectionEx(Func<TItem, TKey> keySelector)
        {
            this.keySelector = keySelector;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedCollectionEx{TKey, TItem}"/>
        /// class that uses the specified equality comparer and creates a lookup dictionary
        /// when the specified threshold is exceeded.
        /// </summary>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="comparer">
        /// The implementation of the <see cref="IEqualityComparer{T}"/> generic
        /// interface to use when comparing keys, or null to use the default equality comparer
        /// for the type of the key, obtained from <see cref="EqualityComparer{T}.Default"/>
        /// </param>
        public KeyedCollectionEx(Func<TItem, TKey> keySelector, IEqualityComparer<TKey> comparer) : base(comparer)
        {
            this.keySelector = keySelector;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedCollectionEx{TKey, TItem}"/>
        /// class that uses the specified equality comparer and creates a lookup dictionary
        /// when the specified threshold is exceeded.
        /// </summary>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="comparer">
        /// The implementation of the <see cref="IEqualityComparer{T}"/> generic
        /// interface to use when comparing keys, or null to use the default equality comparer
        /// for the type of the key, obtained from <see cref="EqualityComparer{T}.Default"/>
        /// </param>
        /// <param name="dictionaryCreationThreshold">
        /// The number of elements the collection can hold without creating a lookup dictionary
        /// (0 creates the lookup dictionary when the first item is added), or –1 to specify
        /// that a lookup dictionary is never created.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">dictionaryCreationThreshold is less than –1.</exception>
        public KeyedCollectionEx(Func<TItem, TKey> keySelector, IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold) : base(comparer, dictionaryCreationThreshold)
        {
            this.keySelector = keySelector;
        }

        /// <summary>
        /// When implemented in a derived class, extracts the key from the specified element.
        /// </summary>
        /// <param name="item">The element from which to extract the key.</param>
        /// <returns>The key for the specified element.</returns>
        protected override TKey GetKeyForItem(TItem item) =>
            this.keySelector(item);
    }
}