using Cauldron.Consoles;
using Cauldron.Localization;
using System;
using System.Globalization;

namespace Win32_Console_ParameterHandling
{
    [ExecutionGroup("mainGroup")]
    public sealed class MainExecutionGroup : IExecutionGroup
    {
        [Parameter("help", "help", "h")]
        public bool Help { get; private set; }

        [Parameter("language", true, "lang", "L")]
        public string Language { get; private set; }

        public void Execute(ParameterParser parser)
        {
            var parameters = parser.GetActiveParameters(this);

            if (parameters.Contains(nameof(Language)))
            {
                if (string.IsNullOrEmpty(this.Language))
                    Console.WriteLine(Locale.Current.CultureInfo.DisplayName);
                else
                {
                    Locale.Current.CultureInfo = new CultureInfo(this.Language);
                    Console.WriteLine(Locale.Current.CultureInfo.DisplayName);
                }
            }

            if (this.Help)
                parser.ShowHelp();
        }
    }
}