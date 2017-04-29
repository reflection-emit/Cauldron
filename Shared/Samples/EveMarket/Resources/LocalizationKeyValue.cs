using Newtonsoft.Json;

namespace EveMarket.Resources
{
    public class LocalizationKeyValue
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("lang")]
        public LocalizationLanguage Language { get; set; }
    }
}