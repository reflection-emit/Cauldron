using System;

namespace Cauldron.Core
{
    /// <summary>
    /// Provides a randomizer that is cryptographicly secure
    /// </summary>
    public static partial class Randomizer
    {
        private readonly static Random local = new Random(GetCryptographicSeed());

        /// <summary>
        /// Returns a nonnegative random number.
        /// Cryptographic secure.
        /// </summary>
        /// <returns>A random integer value</returns>
        public static int Next()
        {
            return local.Next();
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
            return local.Next(minValue, maxValue);
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
            return array[local.Next(0, array.Length)];
        }

        /// <summary>
        /// Returns a random boolean.
        /// Cryptographic secure.
        /// </summary>
        /// <returns>A random boolean</returns>
        public static bool NextBoolean()
        {
            return Next(0, 1000) > 500;
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
            return local.NextDouble();
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
            double randomValue = local.NextDouble();
            return minValue + randomValue * (maxValue - minValue);
        }
    }
}