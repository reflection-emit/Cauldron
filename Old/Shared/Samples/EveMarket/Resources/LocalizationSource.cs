using Cauldron.Activator;
using Cauldron.Localization;

namespace EveMarket.Resources
{
    [Component(typeof(ILocalizationSource), FactoryCreationPolicy.Singleton, 1)]
    public class LocalizationSource : JsonLocalizationSourceBase<LocalizationLanguage>
    {
        [ComponentConstructor]
        public LocalizationSource() : base("strings.json")
        {
        }
    }
}