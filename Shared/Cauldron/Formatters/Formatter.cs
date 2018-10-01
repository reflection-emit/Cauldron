using System;
using System.Collections.Generic;
using System.Globalization;

namespace Cauldron.Formatters
{
    /// <summary>
    /// Represents an extensible format provider <see cref="IFormatProvider"/>
    /// </summary>
    public sealed class Formatter : FormatterBase
    {
        private static List<ICustomFormatter> formatters = new List<ICustomFormatter>();

        static Formatter()
        {
            formatters.AddRange(new FormatterBase[] { new ByteSizeFormatter(), new MetricUnitFormatter() });
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Formatter"/>
        /// </summary>
        /// <param name="cultureInfo">An object that supplies culture-specific formatting information.</param>
        public Formatter(CultureInfo cultureInfo) : base(provider: cultureInfo)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Formatter"/>
        /// </summary>
        public Formatter() : base(CultureInfo.CurrentCulture)
        {
        }

        /// <summary>
        /// Adds a new custom formatter to the collection
        /// </summary>
        /// <param name="customFormatter"></param>
        public static void Add(ICustomFormatter customFormatter) => formatters.Add(customFormatter);

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
            for (int i = 0; i < formatters.Count; i++)
            {
                var result = formatters[i].Format(format, arg, formatProvider);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}