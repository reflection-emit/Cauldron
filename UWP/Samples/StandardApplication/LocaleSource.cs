using Cauldron.Activator;
using Cauldron.Localization;

namespace StandardApplication
{
    [Component(typeof(ILocalizationSource))]
    public class LocaleSource : ILocalizationSource
    {
        public bool Contains(string key, string twoLetterISOLanguageName)
        {
            return true;
        }

        public string GetValue(string key, string twoLetterISOLanguageName)
        {
            return key;
        }
    }
}