using System;

namespace Cauldron.Core.Formatters
{
    internal sealed class Formatter : FormatterBase
    {
        private readonly FormatterBase[] formatters;

        public Formatter(IFormatProvider provider)
        {
            this.formatters = new FormatterBase[] { new ByteSizeFormatter(provider), new MetricUnitFormatter(provider) };
        }

        protected override string OnFormat(string format, object arg, IFormatProvider formatProvider)
        {
            for (int i = 0; i < this.formatters.Length; i++)
            {
                var result = this.formatters[i].Format(format, arg, formatProvider);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}