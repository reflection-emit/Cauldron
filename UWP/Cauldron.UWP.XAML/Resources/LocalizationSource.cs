using Cauldron.Activator;
using Cauldron.Core;
using Cauldron.Core.Extensions;
using Cauldron.Core.Yaml;
using Cauldron.Localization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Cauldron.XAML.Resources
{
    /// <summary>
    /// An implementation of <see cref="ILocalizationSource"/> for Cauldron.XAML
    /// </summary>
    [Component(typeof(ILocalizationSource), FactoryCreationPolicy.Singleton)]
    public sealed class LocalizationSource : ILocalizationSource
    {
        private Dictionary<string, PropertyInfo> language = new Dictionary<string, PropertyInfo>();
        private Dictionary<string, LocalizationKeyValue> localizations = new Dictionary<string, LocalizationKeyValue>();

        [ComponentConstructor]
        internal LocalizationSource()
        {
            try
            {
                foreach (var item in YamlConvert.DeserializeObject<LocalizationKeyValue>(Assemblies.GetManifestResource("7B5BE4E7E11D87C4E23B68589848BB2A.yaml").TryEncode()))
                    this.localizations.Add(item.Key, item);

                foreach (var item in typeof(LocalizationKeyValue).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    this.language.Add(item.GetCustomAttribute<YamlPropertyAttribute>().Name, item);
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
        public string GetValue(string key, string twoLetterISOLanguageName)
        {
            var lang = this.localizations[key];

            if (this.language.ContainsKey(twoLetterISOLanguageName.ToLower()))
            {
                var result = this.language[twoLetterISOLanguageName].GetValue(lang) as string;
                if (string.IsNullOrEmpty(result))
                    return lang.English;

                return result;
            }
            else
                return lang.English;
        }

        private class LocalizationKeyValue
        {
            [YamlProperty("zh")]
            public string Chinese { get; set; }

            [YamlProperty("en")]
            public string English { get; set; }

            [YamlProperty("fr")]
            public string French { get; set; }

            [YamlProperty("de")]
            public string German { get; set; }

            [YamlProperty("el")]
            public string Greek { get; set; }

            [YamlProperty("ja")]
            public string Japanese { get; set; }

            [YamlProperty("key")]
            public string Key { get; set; }

            [YamlProperty("ko")]
            public string Korean { get; set; }

            [YamlProperty("lt")]
            public string Lithuanian { get; set; }

            [YamlProperty("pt")]
            public string Portuguese { get; set; }

            [YamlProperty("ru")]
            public string Russian { get; set; }

            [YamlProperty("es")]
            public string Spanish { get; set; }

            [YamlProperty("sv")]
            public string Swedish { get; set; }

            [YamlProperty("th")]
            public string Thai { get; set; }
        }
    }
}