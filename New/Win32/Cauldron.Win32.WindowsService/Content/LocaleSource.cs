using Cauldron.Activator;
using Cauldron.Localization;

namespace @namespace
{
    [Component(typeof(ILocalizationSource), FactoryCreationPolicy.Singleton, 1)]
    public sealed class LocaleSource : JsonLocalizationSourceBase<LocalizationKeyValue>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="LocaleSource"/>.
        /// </summary>
        public LocaleSource() : base("strings.json")
        {
        }
    }
}