using System;

namespace Cauldron.Core
{
    /// <summary>
    /// Provides a randomizer that is cryptographicly secure
    /// </summary>
    [Factory(typeof(IRandomizer))]
    public partial class Randomizer : Singleton<Randomizer>, IRandomizer
    {
        private readonly Random local;

        /// <summary>
        /// Initializes a new instance of <see cref="Randomizer"/>
        /// </summary>
        public Randomizer()
        {
            this.local = this.CreateRandomInstance();
        }

        /// <summary>
        /// Returns a nonnegative random number.
        /// Cryptographic secure.
        /// </summary>
        /// <returns>A random integer value</returns>
        public int Next() => local.Next();

        /// <summary>
        /// Returns a random number within a specified range.
        /// Cryptographic secure.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated. <paramref name="maxValue"/> must be greater than or equal to 0.</param>
        /// <returns>A random integer value</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue"/> is greater than <paramref name="maxValue"/></exception>
        public int Next(int minValue, int maxValue) => local.Next(minValue, maxValue);

        /// <summary>
        /// Returns a random number item from the array.
        /// Cryptographic secure.
        /// </summary>
        /// <typeparam name="T">The array item type</typeparam>
        /// <param name="array">The array</param>
        /// <returns>A random item from the array</returns>
        public T Next<T>(T[] array) => array[local.Next(0, array.Length)];

        /// <summary>
        /// Returns a random boolean.
        /// Cryptographic secure.
        /// </summary>
        /// <returns>A random boolean</returns>
        public bool NextBoolean() => local.Next(0, 1000) >= 500;

        /// <summary>
        /// Returns a random byte value
        /// </summary>
        /// <returns>A random byte value (0 to 255)</returns>
        public byte NextByte() => (byte)local.Next(0, 256);

        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// Cryptographic secure.
        /// </summary>
        /// <returns>A random value</returns>
        public double NextDouble() => local.NextDouble();

        /// <summary>
        /// Returns a random number within a specified range.
        /// Cryptographic secure.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated. <paramref name="maxValue"/> must be greater than or equal to 0.</param>
        /// <returns>A random value</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue"/> is greater than <paramref name="maxValue"/></exception>
        public double NextDouble(double minValue, double maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException("minValue", minValue, "minValue is greater than maxValue.");

            double randomValue = local.NextDouble();
            return minValue + randomValue * (maxValue - 1 - minValue);
        }

        /// <summary>
        /// Occures on class initialization.
        /// </summary>
        /// <returns>A new instance of the <see cref="Random"/> class</returns>
        protected virtual Random CreateRandomInstance() => new Random(GetCryptographicSeed());
    }
}