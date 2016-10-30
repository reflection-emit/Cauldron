using Cauldron.Activator;
using Cauldron.Core;
using Cauldron.Core.Extensions;
using Cauldron.Localization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace EveMarket.Resources
{
    [Component(typeof(ILocalizationSource), FactoryCreationPolicy.Singleton)]
    public class LocalizationSource : ILocalizationSource
    {
        private Dictionary<string, LocalizationLanguage> localizations = new Dictionary<string, LocalizationLanguage>();

        [ComponentConstructor]
        public LocalizationSource()
        {
            foreach (var item in JsonConvert.DeserializeObject<IEnumerable<LocalizationKeyValue>>(Assemblies.GetManifestResource("strings.json").TryEncode()))
                this.localizations.Add(item.Key, item.Language);
        }

        public bool Contains(string key, string twoLetterISOLanguageName) => this.localizations.ContainsKey(key);

        public string GetValue(string key, string twoLetterISOLanguageName)
        {
            var lang = this.localizations[key];

            if (twoLetterISOLanguageName == "de" && lang.German != null) return string.Join("\r\n", lang.German);
            else return string.Join("\r\n", lang.English); // English is the default language
        }
    }
}