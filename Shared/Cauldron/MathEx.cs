namespace System  /* Make this prominent ... side by side with Math */
{
    /// <summary>
    /// Provides static methods for common mathematical functions.
    /// </summary>
    public static partial class MathEx
    {
        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <param name="min">The minimum value the parameter <paramref name="value"/> can have</param>
        /// <param name="max">The maximum value the parameter <paramref name="value"/> can have</param>
        /// <returns>The clamped value</returns>
        public static double Clamp(double value, double min, double max)
        {
            if (!double.IsNaN(max) && !double.IsInfinity(max) && value > max)
                value = max;

            if (!double.IsNaN(min) && !double.IsInfinity(min) && value < min)
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

        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static double DegreesToRadians(double degrees) => degrees * (Math.PI / 180);

        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static double RadiansToDegrees(double radians) => radians * (180 / Math.PI);

        /// <summary>
        /// Calculates the percentage relationship of two values
        /// </summary>
        /// <param name="value">Current value of the systems</param>
        /// <param name="valueMin">Minimum value of the current system</param>
        /// <param name="valueMax">Maximum value of the current system</param>
        /// <param name="targetValueMin">Minimum value of the target system</param>
        /// <param name="targetValueMax">Maximum value of the target system</param>
        /// <returns></returns>
        public static double ValueOf(double value, double valueMin, double valueMax, double targetValueMin, double targetValueMax)
        {
            var offsetValue = value - valueMin;
            var offsetMax = valueMax - valueMin;
            var offsetTargetMax = targetValueMax - targetValueMin;

            var result = offsetValue * offsetTargetMax / offsetMax;

            return result + targetValueMin;
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