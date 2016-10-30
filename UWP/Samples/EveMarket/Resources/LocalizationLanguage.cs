using Newtonsoft.Json;

namespace EveMarket.Resources
{
    public class LocalizationLanguage
    {
        [JsonProperty("en")]
        public string[] English { get; set; }

        [JsonProperty("de")]
        public string[] German { get; set; }
    }
}