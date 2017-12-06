using Cauldron.Core.Extensions;
using Cauldron.Localization;
using Newtonsoft.Json;

namespace EveMarket.Resources
{
    public class LocalizationLanguage : ILocalizationKeyValue
    {
        [JsonProperty("en")]
        public string[] English { get; set; }

        [JsonProperty("de")]
        public string[] German { get; set; }

        public string Key { get; set; }

        public string GetValue(string twoLetterISOLanguageName)
        {
            if (twoLetterISOLanguageName == "de")
                return this.German?.Join("\r\n") ?? "";

            return this.English?.Join("\r\n") ?? "";
        }
    }
}