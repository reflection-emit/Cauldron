using Couldron.Core;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Couldron
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Determines whether any element of a sequence satisfies a condition.
        /// </summary>
        /// <param name="source">An <see cref="IEnumerable"/> whose elements to apply the predicate to</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>True if any elements in the source sequence pass the test in the specified predicate, otherwise false</returns>
        public static bool Any(this IEnumerable source, Func<object, bool> predicate)
        {
            if (source == null)
                return false;

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
        public static bool Any(this IEnumerable source)
        {
            if (source == null)
                return false;

            foreach (var item in source)
                return true;

            return false;
        }

        /// <summary>
        /// Performs certain types of conversions between compatible reference types or nullable types
        /// <para/>
        /// Returns null if convertion was not successfull
        /// </summary>
        /// <typeparam name="T">The type to convert the <paramref name="target"/> to</typeparam>
        /// <param name="target">The object to convert</param>
        /// <returns>The converted object</returns>
        public static T CastTo<T>(this object target) where T : class
        {
            return target as T;
        }

        /// <summary>
        /// Determines whether an element is in the array
        /// </summary>
        /// <typeparam name="T">The type of elements in the array</typeparam>
        /// <param name="array">The array that could contain the item</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>true if item is found in the array; otherwise, false.</returns>
        public static bool Contains<T>(this T[] array, Func<T, bool> predicate)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (predicate(array[i]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="IEnumerable"/>
        /// </summary>
        /// <param name="target">The <see cref="IEnumerable"/></param>
        /// <returns>The total count of items in the <see cref="IEnumerable"/></returns>
        public static int Count(this IEnumerable target)
        {
            int count = 0;

            foreach (var item in target)
                count++;

            return count;
        }

        /// <summary>
        /// Gets a specified length of bytes.
        /// <para />
        /// If the specified length <paramref name="length"/> is longer than the source array the source array will be returned instead.
        /// </summary>
        /// <param name="target">The Array that contains the data to copy.</param>
        /// <param name="length">A 32-bit integer that represents the number of elements to copy.</param>
        /// <returns>Returns an array of bytes</returns>
        public static byte[] GetBytes(this byte[] target, int length)
        {
            if (length >= target.Length)
                return target;

            byte[] value = new byte[length];

            Array.Copy(target, value, length);
            return value;
        }

        /// <summary>
        /// Gets a specified length of bytes
        /// </summary>
        /// <param name="target">The Array that contains the data to copy.</param>
        /// <param name="startingPosition">A 32-bit integer that represents the index in the sourceArray at which copying begins.</param>
        /// <param name="length">A 32-bit integer that represents the number of elements to copy.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="startingPosition"/> and <paramref name="length"/> are out of range</exception>
        public static byte[] GetBytes(this byte[] target, int startingPosition, int length)
        {
            if (length + startingPosition > target.Length)
                throw new ArgumentOutOfRangeException("length", "Parameter startingPosition and length are out of range");

            byte[] value = new byte[length];

            Array.Copy(target, startingPosition, value, 0, length);
            return value;
        }

        /// <summary>
        /// Searches for the specified byte array and returns the zero-based index of the first
        /// occurrence within the entire <see cref="Array"/>
        /// </summary>
        /// <param name="data">The <see cref="Array"/> that could contain <paramref name="value"/></param>
        /// <param name="value">The object to locate in the <see cref="Array"/>. The value can be null for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire <see cref="Array"/>, if found; otherwise, –1.</returns>
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
        /// <param name="value">The object to locate in the <see cref="Array"/>. The value can be null for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire <see cref="Array"/>, if found; otherwise, –1.</returns>
        public static int IndexOf<T>(this T[] target, T value)
        {
            for (int i = 0; i < target.Length; i++)
                if (target[i].Equals(value))
                    return i;

            return -1;
        }

        /// <summary>
        /// Checks if the value is null. If not, it will invoke <paramref name="action"/>
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="value">The value to check</param>
        /// <param name="action">The action to invoke if <paramref name="value"/> is not null</param>
        public static void IsNotNull<T>(this T value, Action<T> action)
        {
            if (value != null)
                action(value);
        }

        /// <summary>
        /// Returns a string containing a specified number of characters from the left side of a string.
        /// </summary>
        /// <param name="source"><see cref="string"/> expression from which the leftmost characters are returned.</param>
        /// <param name="length">
        /// Numeric expression indicating how many characters to return. If 0, a zero-length string (<see cref="string.Empty"/>) is returned.
        /// If greater than or equal to the number of characters in str, the entire string is returned.
        /// </param>
        /// <returns>Returns a string containing a specified number of characters from the left side of a string.</returns>
        public static string Left(this string source, int length)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            if (length == 0)
                return string.Empty;

            if (source.Length < length || source.Length == length)
                return source;

            return source.Substring(0, length);
        }

        /// <summary>
        /// Moves the specified item to a new location in the collection
        /// </summary>
        /// <typeparam name="T">The Type of item contained in the collection</typeparam>
        /// <param name="source">The source collection that contains the item</param>
        /// <param name="entry">The item to move</param>
        /// <param name="relativeIndex">The new position of the item relativ to its current position.</param>
        public static void Move<T>(this ObservableCollection<T> source, T entry, int relativeIndex)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var entryIndex = source.IndexOf(entry);
            var newIndex = entryIndex + relativeIndex;

            if (newIndex < 0)
                newIndex = 0;

            if (newIndex > source.Count - 1)
                newIndex = source.Count - 1;

            if (entryIndex == newIndex)
                return;

            source.Move(entryIndex, newIndex);
        }

        /// <summary>
        /// Replaces the values of data in memory with random values. The GC handle will be freed.
        /// </summary>
        /// <remarks>Will only work on <see cref="GCHandleType.Pinned"/></remarks>
        /// <param name="target"></param>
        /// <param name="targetLength"></param>
        public static void RandomizeValues(this GCHandle target, int targetLength)
        {
            unsafe
            {
                byte* insecureData = (byte*)target.AddrOfPinnedObject();

                for (int i = 0; i < targetLength; i++)
                    insecureData[i] = Randomizer.NextByte();

                target.Free();
            }
        }

        /// <summary>
        /// Reads all characters from the <see cref="SeekOrigin.Begin"/> to the end of the stream
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read</param>
        /// <returns>The stream as a string</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="stream"/> is null</exception>
        /// <exception cref="NotSupportedException">Parameter <paramref name="stream"/> is not seekable</exception>
        /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        public static string ReadToEnd(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanSeek)
                throw new NotSupportedException("Unseekable streams are not supported");

            stream.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(stream))
            {
                var content = reader.ReadToEnd();
                stream.Dispose();
                return content;
            }
        }

        /// <summary>
        /// Returns a string containing a specified number of characters from the right side of a string.
        /// </summary>
        /// <param name="source"><see cref="string"/> expression from which the rightmost characters are returned.</param>
        /// <param name="length">
        /// Numeric expression indicating how many characters to return. If 0, a zero-length string (<see cref="string.Empty"/>) is returned.
        /// If greater than or equal to the number of characters in str, the entire string is returned.
        /// </param>
        /// <returns>Returns a string containing a specified number of characters from the right side of a string.</returns>
        public static string Right(this string source, int length)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            if (length == 0)
                return string.Empty;

            if (source.Length < length || source.Length == length)
                return source;

            return source.Substring(source.Length - length, length);
        }

        /// <summary>
        /// Converts a <see cref="IEnumerable"/> to an array
        /// </summary>
        /// <typeparam name="T">The type of elements the <see cref="IEnumerable"/> contains</typeparam>
        /// <param name="items">The <see cref="IEnumerable"/> to convert</param>
        /// <returns>An array of <typeparamref name="T"/></returns>
        public static T[] ToArray<T>(this IEnumerable items)
        {
            if (items == null)
                return new T[0];

            T[] result = new T[items.Count()];
            int counter = 0;

            foreach (T item in items)
            {
                result[counter] = item;
                counter++;
            }

            return result;
        }

        /// <summary>
        /// Converts an array of 8-bit unsigned integers to its equivalent string representation
        /// that is encoded with base-64 digits.
        /// </summary>
        /// <param name="target">An array of 8-bit unsigned integers.</param>
        /// <returns>The string representation, in base 64, of the contents of <paramref name="target"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is null.</exception>
        public static string ToBase64String(this byte[] target)
        {
            return Convert.ToBase64String(target);
        }

        /// <summary>
        /// Converts a <see cref="Stream"/> to <see cref="byte"/> array
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to convert</param>
        /// <returns>An array of bytes</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="stream"/> is null</exception>
        /// <exception cref="NotSupportedException">Parameter <paramref name="stream"/> is not seekable</exception>
        public static async Task<byte[]> ToBytesAsync(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanSeek)
                throw new NotSupportedException("Unseekable streams are not supported");

            using (var memoryStream = new MemoryStream())
            {
                memoryStream.SetLength(stream.Length);
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Returns a 32-bit signed integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="target">An array of bytes.</param>
        /// <returns>A 32-bit signed integer formed by four bytes</returns>
        public static int ToInteger(this byte[] target)
        {
            return BitConverter.ToInt32(target, 0);
        }
    }
}