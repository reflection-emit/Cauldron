using Cauldron;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Returns a value indicating whether a specified substring occurs within this string.
        /// </summary>
        /// <param name="text">The string to seek from.</param>
        /// <param name="value">The string to seek.</param>
        /// <param name="comparisonType">
        /// One of the enumeration values that specifies the rules for the search.
        /// </param>
        /// <returns>
        /// True if the value parameter occurs within this string, or if value is the empty string
        /// (""); otherwise, false.
        /// </returns>
        public static bool Contains(this string text, string value, StringComparison comparisonType) =>
            text.IndexOf(value, comparisonType) >= 0;

        /// <summary>
        /// Returns distinct elements from a sequence by using a selector to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/></typeparam>
        /// <param name="source">The sequence to remove duplicate elements from.</param>
        /// <param name="selector">
        /// An expression used to determines whether the specified object are equal
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> that contains distinct elements from the source sequence.
        /// </returns>
        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, bool> selector)
        {
            var comparer = new DynamicEqualityComparer<TSource>(selector);
            return source.Distinct(comparer);
        }

        /// <summary>
        /// Gets the string enclosed by two strings
        /// </summary>
        /// <param name="target">The string that contains the string to be retrieved</param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>The enclosed string</returns>
        public static string EnclosedIn(this string target, string start = "(", string end = ")")
        {
            if (string.IsNullOrEmpty(target))
                return target;

            int startPos = target.IndexOf(start) + start.Length;

            if (startPos < 0)
                return target;

            int endPos = target.IndexOf(end, startPos + 1);

            if (endPos <= startPos)
                endPos = target.Length - 1;

            return target.Substring(startPos, endPos - startPos);
        }

        /// <summary>
        /// Searches for the specified byte array and returns the zero-based index of the first
        /// occurrence within the entire <see cref="Array"/>
        /// </summary>
        /// <param name="data">The <see cref="Array"/> that could contain <paramref name="value"/></param>
        /// <param name="value">
        /// The object to locate in the <see cref="Array"/>. The value can be null for reference types.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the entire <see cref="Array"/>, if found; otherwise, –1.
        /// </returns>
        public static long IndexOf(this byte[] data, byte[] value)
        {
            if (value.Length > data.Length)
                return -1;

            unsafe
            {
                var dataLength = data.Length;
                var findLength = value.Length;
                var unequal = false;

                fixed (byte* pData = data, pFind = value)
                {
                    byte* dataPointer = pData;
                    byte* findPointer = pFind;

                    do
                    {
                        byte* currentDataPointer = dataPointer;

                        do
                        {
                            if (*currentDataPointer != *findPointer)
                            {
                                unequal = true;
                                break;
                            }

                            findPointer++;
                            currentDataPointer++;
                        }
                        while (--findLength > 0);

                        if (!unequal)
                            return dataPointer - pData;

                        unequal = false;
                        // reset the length
                        findLength = value.Length;
                        // move the pointer back to the next byte
                        dataPointer++;
                        // reset the find pointer
                        findPointer = pFind;
                    }
                    while (--dataLength > findLength);
                }
            }

            return -1;
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first
        /// occurrence within the entire <see cref="Array"/>
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Array"/></typeparam>
        /// <param name="target">The <see cref="Array"/> that could contain <paramref name="value"/></param>
        /// <param name="value">
        /// The object to locate in the <see cref="Array"/>. The value can be null for reference types.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the entire <see
        /// cref="Array"/>, if found; otherwise, –1.
        /// </returns>
        public static int IndexOf<T>(this T[] target, T value)
        {
            for (int i = 0; i < target.Length; i++)
                if (target[i].Equals(value))
                    return i;

            return -1;
        }

        /// <summary>
        /// Checkes if the string is encoded in Base64.
        /// </summary>
        /// <param name="str">The string to check</param>
        /// <returns>True if the string is base 64 encoded; otherwise false</returns>
        public static bool IsBase64String(this string str)
        {
            if (str.Replace(" ", "").Length % 4 != 0)
                return false;

            try
            {
                Convert.FromBase64String(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}