using Cauldron.Activator;
using Cauldron.Localization;
using System.Collections.Generic;

namespace Win32_Console_ParameterHandling
{
    [Component(typeof(ILocalizationSource), FactoryCreationPolicy.Singleton)]
    public class LocaleSource : YamlLocalizationSourceBase<LocalizationKeyValue>
    {
        private Dictionary<string, LocalizationKeyValue> localizations = new Dictionary<string, LocalizationKeyValue>();

        [ComponentConstructor]
        public LocaleSource() : base("string.yaml")
        {
        }
    }
}