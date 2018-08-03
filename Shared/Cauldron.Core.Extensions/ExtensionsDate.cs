using Cauldron.Core;
using System;

namespace Cauldron
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
#if PUBLIC

    public static partial class ExtensionsDate
#else
    internal static partial class ExtensionsDate

#endif
    {
        /// <summary>
        /// Returns the maximum of the week of the given year
        /// </summary>
        /// <param name="dateTime">The year to get the maximum weeks.</param>
        /// <returns>The maximum week of the given year</returns>
        public static int GetWeeksInYear(this DateTime dateTime) => Utilities.GetWeeksInYear(dateTime.Year);
    }
}