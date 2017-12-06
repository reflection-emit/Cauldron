using Cauldron.Activator;
using Cauldron.Localization;
using System.Collections.Generic;

namespace InputValidation
{
    [Component(typeof(ILocalizationSource))]
    public sealed class LocaleSource : YamlLocalizationSourceBase<LocalizationKeyValue>
    {
        private Dictionary<string, string> localizations = new Dictionary<string, string>();

        [ComponentConstructor]
        public LocaleSource() : base("validation_text.yaml")
        {
        }
    }
}