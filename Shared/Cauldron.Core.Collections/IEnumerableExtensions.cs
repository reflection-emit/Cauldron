using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Core.Collections
{
    /// <summary>
    /// Provides usefull extensions for <see cref="IEnumerable"/>
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Provides linq like methods for handling and converting <see cref="IEnumerable"/>s.
        /// <para/>
        /// This is separated from actual extension to avoid confusions with <see cref="System.Linq"/> extensions. And
        /// also to avoid accidental usage.
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable"/> of interest.</param>
        /// <returns>A new instance of the <see cref="IEnumerableExtensions"/> class.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null</exception>
        public static IEnumerableExtensions Operations(this IEnumerable source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new IEnumerableExtensions(source);
        }
    }

    /// <summary>
    /// Provides usefull extensions for <see cref="IEnumerable"/>.
    /// </summary>
    public sealed class IEnumerableExtensions
    {
        private IEnumerable source;

        internal IEnumerableExtensions(IEnumerable enumerable)
        {
            this.source = enumerable;
        }

        /// <summary>
        /// Determines whether any element of a sequence satisfies a condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>True if any elements in the source sequence pass the test in the specified predicate, otherwise false</returns>
        public bool Any(Func<object, bool> predicate)
        {
            foreach (var item in this.source)
            {
                if (predicate(item))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <returns>True if the source sequence contains any elements, otherwise false.</returns>
        public bool Any()
        {
            foreach (var item in source)
                return true;

            return false;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="IEnumerable"/>
        /// </summary>
        /// <returns>The total count of items in the <see cref="IEnumerable"/></returns>
        public int Count()
        {
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
        /// <param name="index">The index of the element</param>
        /// <returns>The element with the specified index</returns>
        public object ElementAt(int index)
        {
            var counter = 0;
            foreach (var item in this.source)
            {
                if (counter++ == index)
                    return item;
            }

            throw new ArgumentException("The IEnumerable does not have an index '" + index + "'");
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="IEnumerable"/>
        /// </summary>
        /// <param name="itemToExcept">The object to remove from the <see cref="IEnumerable"/>. The value can be null for reference types.</param>
        /// <returns>A new instance of the <see cref="IEnumerable"/> without the item specified by <paramref name="itemToExcept"/></returns>
        public IEnumerable Except(object itemToExcept)
        {
            var result = new List<object>();

            foreach (var item in this.source)
            {
                if (!Comparer.Equals(item, itemToExcept))
                    result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Returns the first element of a sequence; otherwise null.
        /// </summary>
        /// <returns>The first element in the specified sequence.</returns>
        public object FirstOrDefault()
        {
            var list = this.source as IList;
            if (list != null)
            {
                if (list.Count == 0)
                    return null;

                return list[0];
            }

            var collection = this.source as ICollection;
            if (collection != null && collection.Count == 0)
                return null;

            var array = this.source as Array;
            if (array != null)
            {
                if (array.Length == 0)
                    return null;

                return array.GetValue(0);
            }

            var enumerator = source.GetEnumerator();

            try
            {
                enumerator.Reset();

                if (enumerator.MoveNext())
                    return enumerator.Current;
            }
            catch
            {
                return null;
            }
            finally
            {
                enumerator.TryDispose();
            }
            return null;
        }

        /// <summary>
        /// Determines whether two sequences are equal by comparing the elements by using the default equality comparer for their types
        /// </summary>
        /// <param name="second">An <see cref="IEnumerable"/> to compare to the first sequence.</param>
        /// <returns>
        /// true if the two source sequences are of equal length and their corresponding
        /// elements are equal according to the default equality comparer for their type;
        /// otherwise, false.
        /// </returns>
        public bool SequenceEqual(IEnumerable second)
        {
            if (second == null)
                throw new ArgumentNullException(nameof(second));

            var firstCol = this.source as ICollection;

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

            var e1 = source.GetEnumerator();
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
        /// <returns>An array of <typeparamref name="T"/></returns>
        public T[] ToArray<T>()
        {
            var result = new T[this.Count()];
            var counter = 0;

            foreach (T item in this.source)
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
        /// <returns>A <see cref="List{T}"/> that contains elements from the input sequence.</returns>
        public List<T> ToList<T>()
        {
            var result = new List<T>(this.Count());

            foreach (T item in this.source)
                result.Add(item);

            return result;
        }
    }
}