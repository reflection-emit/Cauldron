using System;

namespace Cauldron.Core
{
    /// <summary>
    /// Represents a randomizer that is cryptographicly secure
    /// </summary>
    public interface IRandomizer
    {
        /// <summary>
        /// Returns a random number within a specified range.
        /// Cryptographic secure.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated. <paramref name="maxValue"/> must be greater than or equal to 0.</param>
        /// <returns>A random integer value</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue"/> is greater than <paramref name="maxValue"/></exception>
        int Next(int minValue, int maxValue);

        /// <summary>
        /// Returns a nonnegative random number.
        /// Cryptographic secure.
        /// </summary>
        /// <returns>A random integer value</returns>
        int Next();

        /// <summary>
        /// Returns a random number item from the array.
        /// Cryptographic secure.
        /// </summary>
        /// <typeparam name="T">The array item type</typeparam>
        /// <param name="array">The array</param>
        /// <returns>A random item from the array</returns>
        T Next<T>(T[] array);

        /// <summary>
        /// Returns a random boolean.
        /// Cryptographic secure.
        /// </summary>
        /// <returns>A random boolean</returns>
        bool NextBoolean();

        /// <summary>
        /// Returns a random byte value
        /// </summary>
        /// <returns>A random byte value (0 to 255)</returns>
        byte NextByte();

        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// Cryptographic secure.
        /// </summary>
        /// <returns>A random value</returns>
        double NextDouble();

        /// <summary>
        /// Returns a random number within a specified range.
        /// Cryptographic secure.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated. <paramref name="maxValue"/> must be greater than or equal to 0.</param>
        /// <returns>A random value</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue"/> is greater than <paramref name="maxValue"/></exception>
        double NextDouble(double minValue, double maxValue);
    }
}