using Cauldron.Activator;
using Cauldron.Localization;

namespace ThemeSample
{
    [Component(typeof(ILocalizationSource), FactoryCreationPolicy.Singleton)]
    public sealed class LocaleSource : JsonLocalizationSourceBase<LocalizationKeyValue>
    {
        public LocaleSource() : base("strings.json")
        {
        }
    }
}