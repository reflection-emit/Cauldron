using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Core.Collections
{
    /// <summary>
    /// Provides usefull extensions for <see cref="IEnumerable"/>
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Determines whether any element of a sequence satisfies a condition.
        /// </summary>
        /// <param name="source">An <see cref="IEnumerable"/> whose elements to apply the predicate to</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>True if any elements in the source sequence pass the test in the specified predicate, otherwise false</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null</exception>
        public static bool Any_(this IEnumerable source, Func<object, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (var item in source)
            {
                if (predicate(item))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable"/> to check for emptiness.</param>
        /// <returns>True if the source sequence contains any elements, otherwise false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null</exception>
        public static bool Any_(this IEnumerable source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (var item in source)
                return true;

            return false;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="IEnumerable"/>
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable"/></param>
        /// <returns>The total count of items in the <see cref="IEnumerable"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null</exception>
        public static int Count_(this IEnumerable source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            int count = 0;

            if (source.GetType().IsArray)
                return (source as Array).Length;

            var collection = source as ICollection;
            if (collection != null)
                return collection.Count;

            var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
                count++;

            enumerator.TryDispose();

            return count;
        }

        /// <summary>
        /// Returns the element at the defined index
        /// </summary>
        /// <param name="ienumerable">The enumerable that contains the element</param>
        /// <param name="index">The index of the element</param>
        /// <returns>The element with the specified index</returns>
        /// <exception cref="ArgumentNullException"><paramref name="ienumerable"/> is null</exception>
        public static object ElementAt_(this IEnumerable ienumerable, int index)
        {
            if (ienumerable == null)
                throw new ArgumentNullException(nameof(ienumerable));

            var counter = 0;
            foreach (var item in ienumerable)
            {
                if (counter++ == index)
                    return item;
            }

            throw new ArgumentException("The IEnumerable does not have an index '" + index + "'");
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="IEnumerable"/>
        /// </summary>
        /// <param name="items">The <see cref="IEnumerable"/> that may contain the object to remove</param>
        /// <param name="itemToExcept">The object to remove from the <see cref="IEnumerable"/>. The value can be null for reference types.</param>
        /// <returns>A new instance of the <see cref="IEnumerable"/> without the item specified by <paramref name="itemToExcept"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is null</exception>
        public static IEnumerable Except_(this IEnumerable items, object itemToExcept)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var result = new List<object>();

            foreach (var item in items)
            {
                if (!Comparer.Equals(item, itemToExcept))
                    result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Returns the first element of a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable"/> to return the first element of.</param>
        /// <returns>The first element in the specified sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null</exception>
        public static object FirstElement_(this IEnumerable source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var enumerator = source.GetEnumerator();
            enumerator.Reset();

            if (enumerator.MoveNext())
                return enumerator.Current;

            return null;
        }

        /// <summary>
        /// Determines whether two sequences are equal by comparing the elements by using the default equality comparer for their types
        /// </summary>
        /// <param name="first">An <see cref="IEnumerable"/> to compare to second.</param>
        /// <param name="second">An <see cref="IEnumerable"/> to compare to the first sequence.</param>
        /// <returns>
        /// true if the two source sequences are of equal length and their corresponding
        /// elements are equal according to the default equality comparer for their type;
        /// otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">first or second is null.</exception>
        public static bool SequenceEqual_(this IEnumerable first, IEnumerable second)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));

            if (second == null)
                throw new ArgumentNullException(nameof(second));

            var firstCol = first as ICollection;

            if (firstCol != null)
            {
                var secondCol = second as ICollection;

                if (secondCol != null && firstCol.Count != secondCol.Count)
                    return false;
            }

            /*
                Because we never know what is inside the IEnumerable, we have to assume that they are a colourful mix of any type.
                This means that this will be a very ineffective and slow comparisson, because we have to check every element for its type
                before we try to compare them.
            */

            var e1 = first.GetEnumerator();
            var e2 = second.GetEnumerator();
            e1.Reset();
            e2.Reset();

            while (e1.MoveNext())
                if (!(e2.MoveNext() && Comparer.Equals(e1.Current, e2.Current)))
                    return false;

            return !e2.MoveNext();
        }

        /// <summary>
        /// Converts a <see cref="IEnumerable"/> to an array
        /// </summary>
        /// <typeparam name="T">The type of elements the <see cref="IEnumerable"/> contains</typeparam>
        /// <param name="items">The <see cref="IEnumerable"/> to convert</param>
        /// <returns>An array of <typeparamref name="T"/></returns>
        public static T[] ToArray_<T>(this IEnumerable items)
        {
            if (items == null)
                return new T[0];

            T[] result = new T[items.Count_()];
            int counter = 0;

            foreach (T item in items)
            {
                result[counter] = item;
                counter++;
            }

            return result;
        }

        /// <summary>
        /// Creates a <see cref="List{T}"/> from an <see cref="IEnumerable"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="target">The <see cref="IEnumerable"/> to create a <see cref="List{T}"/> from.</param>
        /// <returns>A System.Collections.Generic.List`1 that contains elements from the input sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is null</exception>
        public static List<T> ToList_<T>(this IEnumerable target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            List<T> result = new List<T>(target.Count_());

            foreach (T x in target)
                result.Add(x);

            return result;
        }
    }
}