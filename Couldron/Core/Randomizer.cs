using System;
using System.Security.Cryptography;

namespace Couldron.Core
{
    /// <summary>
    /// Provides a randomizer that is cryptographicly secure
    /// </summary>
    public static class Randomizer
    {
        private static RNGCryptoServiceProvider _cryptoGlobal = new RNGCryptoServiceProvider();

        [ThreadStatic]
        private static Random _local;

        /// <summary>
        /// Returns a nonnegative random number.
        /// Cryptographic secure.
        /// </summary>
        /// <returns>A random integer value</returns>
        public static int Next()
        {
            if (_local == null)
                _local = new Random(GetCryptographicSeed());

            return _local.Next();
        }

        /// <summary>
        /// Returns a random number within a specified range.
        /// Cryptographic secure.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated.maxValue must be greater than or equal to 0.</param>
        /// <returns>A random integer value</returns>
        public static int Next(int minValue, int maxValue)
        {
            if (_local == null)
                _local = new Random(GetCryptographicSeed());

            return _local.Next(minValue, maxValue);
        }

        /// <summary>
        /// Returns a random number item from the array.
        /// Cryptographic secure.
        /// </summary>
        /// <typeparam name="T">The array item type</typeparam>
        /// <param name="array">The array</param>
        /// <returns>A random item from the array</returns>
        public static T Next<T>(T[] array)
        {
            if (_local == null)
                _local = new Random(GetCryptographicSeed());

            return array[_local.Next(0, array.Length)];
        }

        /// <summary>
        /// Returns a random byte value
        /// </summary>
        /// <returns>A random byte value (0 to 255)</returns>
        public static byte NextByte()
        {
            return (byte)Next(0, 255);
        }

        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// Cryptographic secure.
        /// </summary>
        /// <returns>A random value</returns>
        public static double NextDouble()
        {
            if (_local == null)
            {
                _local = new Random(GetCryptographicSeed());
            }

            return _local.NextDouble();
        }

        /// <summary>
        /// Returns a random number within a specified range.
        /// Cryptographic secure.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated.maxValue must be greater than or equal to 0.</param>
        /// <returns>A random value</returns>
        public static double NextDouble(double minValue, double maxValue)
        {
            if (_local == null)
                _local = new Random(GetCryptographicSeed());

            double randomValue = _local.NextDouble();
            return minValue + randomValue * (maxValue - minValue);
        }

        private static int GetCryptographicSeed()
        {
            byte[] buffer = new byte[4];

            _cryptoGlobal.GetBytes(buffer);

            return BitConverter.ToInt32(buffer, 0);
        }
    }
}