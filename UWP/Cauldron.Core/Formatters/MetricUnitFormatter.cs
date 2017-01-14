using Cauldron.Core.Extensions;
using System;

namespace Cauldron.Core.Formatters
{
    /// <summary>
    /// Formats a numeric value to a human readable metric number.
    /// <para/>
    /// For example: The reformatted value of the number 2400 is 2.4k
    /// </summary>
    /// <example>
    /// <code>
    /// var result = string.Format(new MetricUnitFormatter(), "The size is {0:metric}", value);
    /// var result = string.Format(new MetricUnitFormatter(), "The size is {0:metric #0.00}", value);
    /// </code>
    /// </example>
    public sealed class MetricUnitFormatter : FormatterBase
    {
        private const string metricKey = "metric";

        /// <summary>
        /// Initializes a new instance of <see cref="MetricUnitFormatter"/>
        /// </summary>
        public MetricUnitFormatter()
        {
        }

        internal MetricUnitFormatter(IFormatProvider provider) : base(provider)
        {
        }

        /// <summary>
        /// Converts the value of a specified object to an equivalent string representation using specified format and culture-specific formatting information.
        /// </summary>
        /// <param name="format">A format string containing formatting specifications. </param>
        /// <param name="arg">An object to format. </param>
        /// <param name="formatProvider">An object that supplies format information about the current instance. </param>
        /// <returns>The string representation of the value of arg, formatted as specified by format and formatProvider.</returns>
        protected override string OnFormat(string format, object arg, IFormatProvider formatProvider)
        {
            // Origin: http://stackoverflow.com/questions/12181024/formatting-a-number-with-a-metric-prefix by Thom Smith

            if (format.StartsWith(metricKey))
            {
                var value = arg.ToString().ToDouble();

                if (double.IsNaN(value))
                    return "NaN";

                var formatter = "#0.00";

                if (format.IndexOf(' ') > 0)
                    formatter = format.Split(' ')[1];

                if (value == 0)
                    return value.ToString(formatter, formatProvider);

                var incPrefixes = new char[] { 'k', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y' };
                var decPrefixes = new char[] { 'm', 'µ', 'n', 'p', 'f', 'a', 'z', 'y' };

                var degree = Math.Min((int)Math.Floor(Math.Log10(Math.Abs(value)) / 3), incPrefixes.Length - 1);
                var scaled = value * Math.Pow(1000, -degree);

                char? prefix = null;

                switch (Math.Sign(degree))
                {
                    case 1:
                        prefix = incPrefixes[degree - 1];
                        break;

                    case -1:
                        prefix = decPrefixes[-degree - 1];
                        break;
                }

                return scaled.ToString(formatter, formatProvider) + " " + prefix;
            }

            return null;
        }
    }
}