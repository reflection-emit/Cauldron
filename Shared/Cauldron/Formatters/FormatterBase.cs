using System;
using System.Globalization;

namespace Cauldron.Formatters
{
    /// <summary>
    /// Provides a base class for custom formatters
    /// </summary>
    public abstract class FormatterBase : IFormatProvider, ICustomFormatter
    {
        private readonly IFormatProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatterBase"/>
        /// </summary>
        public FormatterBase()
        {
        }

        internal FormatterBase(IFormatProvider provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Converts the value of a specified object to an equivalent string representation using specified format and culture-specific formatting information.
        /// </summary>
        /// <param name="format">A format string containing formatting specifications. </param>
        /// <param name="arg">An object to format. </param>
        /// <param name="formatProvider">An object that supplies format information about the current instance. </param>
        /// <returns>The string representation of the value of arg, formatted as specified by format and formatProvider.</returns>
        public string Format(string format, object arg, IFormatProvider formatProvider) =>
            this.OnFormat(format, arg, this.provider == null ? formatProvider : this.provider);

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

        /// <summary>
        /// Converts the value of a specified object to an equivalent string representation using specified format and culture-specific formatting information.
        /// </summary>
        /// <param name="format">A format string containing formatting specifications. </param>
        /// <param name="arg">An object to format. </param>
        /// <param name="formatProvider">An object that supplies format information about the current instance. </param>
        /// <returns>The string representation of the value of arg, formatted as specified by format and formatProvider.</returns>
        protected abstract string OnFormat(string format, object arg, IFormatProvider formatProvider);
    }
}