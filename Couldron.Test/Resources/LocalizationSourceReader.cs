using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Couldron.Test.Resources
{
    [Factory(typeof(ILocalizationSource), FactoryCreationPolicy.Singleton)]
    public class LocalizationSourceReader : ILocalizationSource
    {
        private Dictionary<string, LocalizationLanguage> localizations = new Dictionary<string, LocalizationLanguage>();

        public LocalizationSourceReader()
        {
            foreach (var item in JsonConvert.DeserializeObject<IEnumerable<LocalizationKeyValue>>(AssemblyUtil.GetManifestResourceStream("strings.json").ReadToEnd()))
                this.localizations.Add(item.Key, item.Language);
        }

        public bool Contains(string key, string twoLetterISOLanguageName)
        {
            return this.localizations.ContainsKey(key);
        }

        public string GetValue(string key, string twoLetterISOLanguageName)
        {
            var lang = this.localizations[key];

            if (twoLetterISOLanguageName == "de")
                return lang.German;
            else
                return lang.English;
        }

        public class LocalizationKeyValue
        {
            [JsonProperty("key")]
            public string Key { get; set; }

            [JsonProperty("lang")]
            public LocalizationLanguage Language { get; set; }
        }

        public class LocalizationLanguage
        {
            [JsonProperty("en")]
            public string English { get; set; }

            [JsonProperty("de")]
            public string German { get; set; }
        }
    }
}