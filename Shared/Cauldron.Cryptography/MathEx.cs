namespace Cauldron.Cryptography
{
    internal static class MathEx
    {
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

        /// <summary>
        /// Calculates the percentage relationship of two values
        /// </summary>
        /// <param name="value">Current value of the systems</param>
        /// <param name="valueMax">Maximum value of the current system</param>
        /// <param name="targetValueMax">Minimum value of the target system</param>
        /// <returns></returns>
        public static double ValueOf(double value, double valueMax, double targetValueMax) => value * targetValueMax / valueMax;
    }
}