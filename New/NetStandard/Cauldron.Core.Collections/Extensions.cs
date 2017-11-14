using Cauldron.Core.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cauldron
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Creates a <see cref="KeyedCollection{TKey, TItem}"/> from an <see cref="IEnumerable{T}"/>
        /// according to a specified key selector function.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">
        /// An <see cref="IEnumerable{T}"/> to create a <see cref="KeyedCollection{TKey, TItem}"/> from.
        /// </param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <returns>
        /// A <see cref="KeyedCollection{TKey, TItem}"/> that contains values of <paramref name="source"/>.
        /// </returns>
        public static KeyedCollection<TKey, TSource> ToKeyedCollection<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var collection = new KeyedCollectionEx<TKey, TSource>(keySelector);

            foreach (var item in source)
                collection.Add(item);

            return collection;
        }
    }
}