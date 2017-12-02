using System;

#if WINDOWS_UWP

using Windows.Security.Cryptography;

#else

using System.Security.Cryptography;

#endif

namespace Cauldron.Core
{
    /// <summary>
    /// Provides a randomizer that is cryptographicly secure
    /// </summary>
    public static class Randomizer
    {
        private readonly static Random local = new Random(GetCryptographicSeed());

        /// <summary>
        /// Returns a nonnegative random number. Cryptographic secure.
        /// </summary>
        /// <returns>A random integer value</returns>
        public static int Next() => local.Next();

        /// <summary>
        /// Returns a random number within a specified range. Cryptographic secure.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">
        /// The exclusive upper bound of the random number to be generated.maxValue must be greater
        /// than or equal to 0.
        /// </param>
        /// <returns>A random integer value</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="minValue"/> is greater than <paramref name="maxValue"/>
        /// </exception>
        public static int Next(int minValue, int maxValue) => local.Next(minValue, maxValue);

        /// <summary>
        /// Returns a random number item from the array. Cryptographic secure.
        /// </summary>
        /// <typeparam name="T">The array item type</typeparam>
        /// <param name="array">The array</param>
        /// <returns>A random item from the array</returns>
        public static T Next<T>(T[] array) => array[local.Next(0, array.Length)];

        /// <summary>
        /// Returns a random <see cref="DateTime"/> between two dates.
        /// </summary>
        /// <param name="from">The inclusive lower bound of the random date returned.</param>
        /// <param name="to">The exclusive upper bound of the random date to be generated.</param>
        /// <returns>A random date</returns>
        public static DateTime Next(DateTime from, DateTime to) =>
            from + (new TimeSpan((long)NextDouble(0.0, (to.AddDays(1) - from).Ticks)));

        /// <summary>
        /// Returns a random boolean. Cryptographic secure.
        /// </summary>
        /// <returns>A random boolean</returns>
        public static bool NextBoolean() => Next(0, 1000) >= 500;

        /// <summary>
        /// Returns a random byte value
        /// </summary>
        /// <returns>A random byte value (0 to 255)</returns>
        public static byte NextByte() => (byte)Next(0, 256);

        /// <summary>
        /// Returns a random number between 0.0 and 1.0. Cryptographic secure.
        /// </summary>
        /// <returns>A random value</returns>
        public static double NextDouble() => local.NextDouble();

        /// <summary>
        /// Returns a random number within a specified range. Cryptographic secure.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">
        /// The exclusive upper bound of the random number to be generated. maxValue must be greater
        /// than or equal to 0.
        /// </param>
        /// <returns>A random value</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="minValue"/> is greater than <paramref name="maxValue"/>
        /// </exception>
        public static double NextDouble(double minValue, double maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException("minValue", minValue, "minValue is greater than maxValue.");

            double randomValue = local.NextDouble();
            return minValue + randomValue * (maxValue - 1 - minValue);
        }

        private static int GetCryptographicSeed()
        {
#if WINDOWS_UWP
            return (int)CryptographicBuffer.GenerateRandomNumber();
#elif NETCORE
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                byte[] buffer = new byte[4];
                randomNumberGenerator.GetBytes(buffer);
                return BitConverter.ToInt32(buffer, 0);
            }
#else
            var cryptoGlobal = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[4];
            // Fills an array of bytes with a cryptographically strong sequence of random values
            cryptoGlobal.GetBytes(buffer);
            return BitConverter.ToInt32(buffer, 0);
#endif
        }
    }
}