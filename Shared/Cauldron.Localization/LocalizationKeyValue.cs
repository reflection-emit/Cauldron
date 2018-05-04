using System;
using Cauldron.Core.Yaml;
using Newtonsoft.Json;

namespace Cauldron.Localization
{
    /// <summary>
    /// Represents a base class for the <see cref="ILocalizationSource"/> dictionary item
    /// </summary>
    public class LocalizationKeyValue : ILocalizationKeyValue
    {
        /// <exclude/>
        [YamlProperty("zh")]
        [JsonProperty("zh")]
        public string Chinese { get; set; }

        /// <exclude/>
        [YamlProperty("en")]
        [JsonProperty("en")]
        public string English { get; set; }

        /// <exclude/>
        [YamlProperty("fr")]
        [JsonProperty("fr")]
        public string French { get; set; }

        /// <exclude/>
        [YamlProperty("de")]
        [JsonProperty("de")]
        public string German { get; set; }

        /// <exclude/>
        [YamlProperty("el")]
        [JsonProperty("el")]
        public string Greek { get; set; }

        /// <exclude/>
        [YamlProperty("ja")]
        [JsonProperty("ja")]
        public string Japanese { get; set; }

        /// <exclude/>
        [YamlProperty("key")]
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <exclude/>
        [YamlProperty("ko")]
        [JsonProperty("ko")]
        public string Korean { get; set; }

        /// <exclude/>
        [YamlProperty("lt")]
        [JsonProperty("lt")]
        public string Lithuanian { get; set; }

        /// <exclude/>
        [YamlProperty("pt")]
        [JsonProperty("pt")]
        public string Portuguese { get; set; }

        /// <exclude/>
        [YamlProperty("ru")]
        [JsonProperty("ru")]
        public string Russian { get; set; }

        /// <exclude/>
        [YamlProperty("es")]
        [JsonProperty("es")]
        public string Spanish { get; set; }

        /// <exclude/>
        [YamlProperty("sv")]
        [JsonProperty("sv")]
        public string Swedish { get; set; }

        /// <exclude/>
        [YamlProperty("th")]
        [JsonProperty("th")]
        public string Thai { get; set; }

        /// <summary>
        /// Gets the default language localization
        /// </summary>
        protected virtual string Default { get { return this.English; } }

        /// <summary>
        /// Gets the localized value of the key
        /// </summary>
        /// <param name="twoLetterISOLanguageName">The two letter iso language name according to ISO</param>
        /// <returns>The localized string.</returns>
        public string GetValue(string twoLetterISOLanguageName)
        {
            var result = this.OnGetValue(twoLetterISOLanguageName);
            if (string.IsNullOrEmpty(result))
                return this.Default;

            return result;
        }

        /// <summary>
        /// Occures when the <see cref="GetValue(string)"/> method has been invoked
        /// </summary>
        /// <param name="twoLetterISOLanguageName">The two letter iso language name according to ISO</param>
        /// <returns>The localized string.</returns>
        protected virtual string OnGetValue(string twoLetterISOLanguageName)
        {
            switch (twoLetterISOLanguageName)
            {
                case "zh": return this.Chinese;
                case "en": return this.English;
                case "fr": return this.French;
                case "de": return this.German;
                case "el": return this.Greek;
                case "ja": return this.Japanese;
                case "ko": return this.Korean;
                case "lt": return this.Lithuanian;
                case "pt": return this.Portuguese;
                case "ru": return this.Russian;
                case "es": return this.Spanish;
                case "sv": return this.Swedish;
                case "th": return this.Thai;

                default: return this.Default;
            }
        }
    }
}