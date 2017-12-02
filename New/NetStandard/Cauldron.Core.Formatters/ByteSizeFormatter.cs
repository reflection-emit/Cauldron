using System;
using System.Globalization;

namespace Cauldron.Core.Formatters
{
    /// <summary>
    /// Formats a numeric value to a human readable size.
    /// <para/>
    /// For example: The reformatted value of the number 2048 is 2KB
    /// </summary>
    /// <example>
    /// <code>
    /// var result = string.Format(new ByteSizeFormatter(), "The size is {0:byte}", value);
    /// var result = string.Format(new ByteSizeFormatter(), "The size is {0:byte 0.###}", value);
    /// </code>
    /// </example>
    public sealed class ByteSizeFormatter : FormatterBase
    {
        private const string metricKey = "byte";

        /// <summary>
        /// Initializes a new instance of <see cref="ByteSizeFormatter"/>
        /// </summary>
        public ByteSizeFormatter()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ByteSizeFormatter"/>
        /// </summary>
        /// <param name="cultureInfo">An object that supplies culture-specific formatting information</param>
        public ByteSizeFormatter(CultureInfo cultureInfo) : base(cultureInfo)
        {
        }

        /// <summary>
        /// Converts the value of a specified object to an equivalent string representation using
        /// specified format and culture-specific formatting information.
        /// </summary>
        /// <param name="format">A format string containing formatting specifications.</param>
        /// <param name="arg">An object to format.</param>
        /// <param name="formatProvider">
        /// An object that supplies format information about the current instance.
        /// </param>
        /// <returns>
        /// The string representation of the value of arg, formatted as specified by format and formatProvider.
        /// </returns>
        protected override string OnFormat(string format, object arg, IFormatProvider formatProvider)
        {
            if (format.StartsWith(metricKey))
            {
                long value;

                if (!long.TryParse(arg?.ToString() ?? "0", out value))
                    value = Convert.ToInt64(arg);

                var formatter = "0.###";

                if (format.IndexOf(' ') > 0)
                    formatter = format.Split(' ')[1];

                // http://www.somacon.com/p576.php

                // Get absolute value
                var absolute_i = (value < 0 ? -value : value);
                // Determine the suffix and readable value
                string suffix;
                double readable;

                if (absolute_i >= 0x1000000000000000) // Exabyte
                {
                    suffix = "EB";
                    readable = (value >> 50);
                }
                else if (absolute_i >= 0x4000000000000) // Petabyte
                {
                    suffix = "PB";
                    readable = (value >> 40);
                }
                else if (absolute_i >= 0x10000000000) // Terabyte
                {
                    suffix = "TB";
                    readable = (value >> 30);
                }
                else if (absolute_i >= 0x40000000) // Gigabyte
                {
                    suffix = "GB";
                    readable = (value >> 20);
                }
                else if (absolute_i >= 0x100000) // Megabyte
                {
                    suffix = "MB";
                    readable = (value >> 10);
                }
                else if (absolute_i >= 0x400) // Kilobyte
                {
                    suffix = "KB";
                    readable = value;
                }
                else // Byte
                    return value.ToString(formatter, formatProvider) + " B";

                // Divide by 1024 to get fractional value
                readable = (readable / 1024);
                // Return formatted number with suffix
                return readable.ToString(formatter, formatProvider) + " " + suffix;
            }

            return null;
        }
    }
}