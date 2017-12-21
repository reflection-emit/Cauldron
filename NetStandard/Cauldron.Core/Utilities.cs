using System;
using System.Globalization;

namespace Cauldron.Core
{
    /// <summary>
    /// Provides useful methods regarding <see cref="DateTime"/>
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Returns the <see cref="DateTime"/> representation of the year and week of year
        /// </summary>
        /// <param name="year">The year to convert</param>
        /// <param name="weekOfYear">The week of year to convert</param>
        /// <exception cref="ArgumentOutOfRangeException">Argument <paramref name="weekOfYear"/> is more than the given year has weeks</exception>
        /// <exception cref="ArgumentOutOfRangeException">Argument <paramref name="weekOfYear"/> is lower than 0</exception>
        /// <returns></returns>
        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            // http://stackoverflow.com/questions/662379/calculate-date-from-week-number
            var jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            var firstThursday = jan1.AddDays(daysOffset);
            var calendar = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = calendar.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            if (weekOfYear > GetWeeksInYear(year))
                throw new ArgumentOutOfRangeException("Argument weekOfYear is more than the given year has weeks");

            if (weekOfYear <= 0)
                throw new ArgumentOutOfRangeException("Argument weekOfYear is lower than 0");

            var weekNum = weekOfYear;

            if (firstWeek <= 1)
                weekNum -= 1;

            var result = firstThursday.AddDays(weekNum * 7);

            return result.AddDays(-3);
        }

        /// <summary>
        /// Returns the maximum of the week of the given year
        /// </summary>
        /// <param name="year">The year to get the maximum weeks</param>
        /// <returns>The maximum week of the given year</returns>
        public static int GetWeeksInYear(int year)
        {
            var dateTimeInfo = DateTimeFormatInfo.CurrentInfo;
            var date1 = new DateTime(year, 12, 31);
            var calendar = dateTimeInfo.Calendar;

            return calendar.GetWeekOfYear(date1, dateTimeInfo.CalendarWeekRule, dateTimeInfo.FirstDayOfWeek);
        }
    }
}