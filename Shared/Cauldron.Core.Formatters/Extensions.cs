using Cauldron.Core.Formatters;
using System.Globalization;

namespace Cauldron
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Converts the value of this instance to its equivalent string representation, using the
        /// specified format.
        /// <para/>
        /// The following custom formatter are already added: <see cref="ByteSizeFormatter"/>, <see cref="MetricUnitFormatter"/>
        /// </summary>
        /// <param name="obj">The object to convert to string</param>
        /// <param name="format">A standard or custom format string</param>
        /// <returns>
        /// The string representation of the value of this instance as specified by format.
        /// </returns>
        public static string ToStringEx(this object obj, string format) =>
            obj.ToStringEx(format, CultureInfo.CurrentCulture);

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation, using the
        /// specified format.
        /// <para/>
        /// The following custom formatter are already added: <see cref="ByteSizeFormatter"/>, <see cref="MetricUnitFormatter"/>
        /// </summary>
        /// <param name="obj">The object to convert to string</param>
        /// <param name="format">A standard or custom format string</param>
        /// <param name="cultureInfo">An object that supplies culture-specific formatting information</param>
        /// <returns>
        /// The string representation of the value of this instance as specified by format.
        /// </returns>
        public static string ToStringEx(this object obj, string format, CultureInfo cultureInfo) =>
            string.Format(new Formatter(cultureInfo), "{0:" + format.Replace("{", "").Replace("}", "") + "}", obj);
    }
}