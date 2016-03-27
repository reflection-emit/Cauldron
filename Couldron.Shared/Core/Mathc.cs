namespace Couldron.Core
{
    /// <summary>
    /// Provides static methods for common mathematical functions.
    /// </summary>
    public static class Mathc
    {        /// <summary>
             /// Clamps a value between a minimum and maximum value.
             /// </summary>
             /// <param name="value">The value to clamp</param>
             /// <param name="min">The minimum value the parameter <paramref name="value"/> can have</param>
             /// <param name="max">The maximum value the parameter <paramref name="value"/> can have</param>
             /// <returns>The clamped value</returns>
        public static double Clamp(double value, double min, double max)
        {
            if (value > max)
                value = max;

            if (value < min)
                value = min;

            return value;
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <param name="min">The minimum value the parameter <paramref name="value"/> can have</param>
        /// <param name="max">The maximum value the parameter <paramref name="value"/> can have</param>
        /// <returns>The clamped value</returns>
        public static int Clamp(int value, int min, int max)
        {
            if (value > max)
                value = max;

            if (value < min)
                value = min;

            return value;
        }
    }
}