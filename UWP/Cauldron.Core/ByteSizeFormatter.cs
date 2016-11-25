using Cauldron.Core.Extensions;
using System;

namespace Cauldron.Core
{
    /// <summary>
    /// Formats a numeric value to a human readable size
    /// </summary>
    public sealed class ByteSizeFormatter : IFormatProvider, ICustomFormatter
    {
        /// <summary>
        /// Converts the value of a specified object to an equivalent string representation using specified format and culture-specific formatting information.
        /// </summary>
        /// <param name="format">A format string containing formatting specifications. </param>
        /// <param name="arg">An object to format. </param>
        /// <param name="formatProvider">An object that supplies format information about the current instance. </param>
        /// <returns>The string representation of the value of arg, formatted as specified by format and formatProvider.</returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            // Check whether this is an appropriate callback
            if (!this.Equals(formatProvider))
                return null;

            // Set default format specifier
            if (string.IsNullOrEmpty(format))
                format = "B";

            if (format == "B")
                return this.GetBytesReadable(arg.ToString().ToLong());
            else
                throw new FormatException(string.Format("The {0} format specifier is invalid.", format));
        }

        /// <summary>
        /// Returns an object that provides formatting services for the specified type.
        /// </summary>
        /// <param name="formatType">An object that specifies the type of format object to return.</param>
        /// <returns>An instance of the object specified by formatType, if the IFormatProvider implementation can supply that type of object; otherwise, null.</returns>
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }

        private string GetBytesReadable(long value)
        {
            // http://www.somacon.com/p576.php

            // Get absolute value
            long absolute_i = (value < 0 ? -value : value);
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
            else
                return value.ToString("0 B"); // Byte

            // Divide by 1024 to get fractional value
            readable = (readable / 1024);
            // Return formatted number with suffix
            return readable.ToString("0.### ") + suffix;
        }
    }
}