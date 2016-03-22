using System;

namespace Couldron
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static class ExtensionConvertions
    {
        /// <summary>
        /// Tries to convert an <see cref="object"/> to a <see cref="bool"/>
        /// </summary>
        /// <param name="target">The object to convert</param>
        /// <returns>Returns a bool that represents the converted object</returns>
        public static bool ToBool(this object target)
        {
            if (target == null)
                return false;

            return target.ToString().ToBool();
        }

        /// <summary>
        /// Tries to convert a <see cref="string"/> to a <see cref="bool"/>
        /// </summary>
        /// <param name="target">The string to convert</param>
        /// <returns>Returns a bool that represents the converted string</returns>
        public static bool ToBool(this string target)
        {
            if (target.IndexOf(bool.TrueString, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                target == "1")
                return true;

            bool result = false;

            if (bool.TryParse(target, out result))
                return result;

            return false;
        }

        /// <summary>
        /// Tries to convert a <see cref="object"/> to an <see cref="int"/>
        /// </summary>
        /// <param name="target">The object to convert</param>
        /// <returns>Returns an int that represents the converted object</returns>
        public static int ToInteger(this object target)
        {
            if (target == null)
                return -1;

            return target.ToString().ToInteger();
        }

        /// <summary>
        /// Tries to convert a <see cref="string"/> to an <see cref="int"/>
        /// </summary>
        /// <param name="target">The string to convert</param>
        /// <returns>Returns an int that represents the converted string</returns>
        public static int ToInteger(this string target)
        {
            int result;

            if (int.TryParse(target, out result))
                return result;

            return -1;
        }
    }
}