using Cauldron.Activator;
using Cauldron.Core;
using Cauldron.Core.Extensions;
using Cauldron.Core.Yaml;
using Cauldron.Localization;
using System.Collections.Generic;

namespace SampleConsoleApplication
{
    [Component(typeof(ILocalizationSource))]
    public class LocaleSource : ILocalizationSource
    {
        private Dictionary<string, LocalizationKeyValue> localizations = new Dictionary<string, LocalizationKeyValue>();

        [ComponentConstructor]
        public LocaleSource()
        {
            foreach (var item in YamlConvert.DeserializeObject<LocalizationKeyValue>(Assemblies.GetManifestResource("string.yaml").TryEncode()))
                this.localizations.Add(item.Key, item);
        }

        /// <summary>
        /// Determines whether an element is in the <see cref="ILocalizationSource"/>
        /// </summary>
        /// <param name="key">The key of the localized string</param>
        /// <param name="twoLetterISOLanguageName">The two letter iso language name according to ISO</param>
        /// <returns>true if item is found in the <see cref="ILocalizationSource"/>; otherwise, false.</returns>
        public bool Contains(string key, string twoLetterISOLanguageName) => this.localizations.ContainsKey(key);

        /// <summary>
        /// Returns a localized value associated with the key
        /// </summary>
        /// <param name="key">The key of the localized string data</param>
        /// <param name="twoLetterISOLanguageName">The two letter iso language name according to ISO</param>
        /// <returns>A <see cref="string"/></returns>
        public string GetValue(string key, string twoLetterISOLanguageName)
        {
            var lang = this.localizations[key];

            if (twoLetterISOLanguageName == "de")
                return lang.German;
            else
                return lang.English;
        }

        private class LocalizationKeyValue
        {
            [YamlProperty("en")]
            public string English { get; set; }

            [YamlProperty("de")]
            public string German { get; set; }

            [YamlProperty("key")]
            public string Key { get; set; }
        }
    }
}