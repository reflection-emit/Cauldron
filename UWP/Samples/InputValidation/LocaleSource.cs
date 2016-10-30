using Cauldron.Activator;
using Cauldron.Core;
using Cauldron.Core.Extensions;
using Cauldron.Core.Yaml;
using Cauldron.Localization;
using System;
using System.Collections.Generic;

namespace InputValidation
{
    [Component(typeof(ILocalizationSource))]
    public sealed class LocaleSource : ILocalizationSource
    {
        private Dictionary<string, string> localizations = new Dictionary<string, string>();

        [ComponentConstructor]
        internal LocaleSource()
        {
            try
            {
                foreach (var item in YamlConvert.DeserializeObject<LocalizationKeyValue>(Assemblies.GetManifestResource("validation_text.yaml").TryEncode()))
                    this.localizations.Add(item.Key, item.Value);
            }
            catch
            {
                throw;
            }
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
        public string GetValue(string key, string twoLetterISOLanguageName) => this.localizations[key];

        private class LocalizationKeyValue
        {
            [YamlProperty("key")]
            public string Key { get; set; }

            [YamlProperty("keyValue")]
            public string Value { get; set; }
        }
    }
}