using Newtonsoft.Json;

namespace EveOnlineApi.Models
{
    public sealed class Motd
    {
        [JsonProperty("incursions")]
        public Next Incursions { get; private set; }

        [JsonProperty("itemTypes")]
        public Next InventoryTypes { get; private set; }

        [JsonProperty("marketPrices")]
        public Next MarketPrices { get; private set; }

        [JsonProperty("marketTypes")]
        public Next MarketTypes { get; private set; }

        [JsonProperty("regions")]
        public Next Regions { get; private set; }

        [JsonProperty("serverName")]
        public string ServerName { get; private set; }

        [JsonProperty("serverVersion")]
        public string ServerVersion { get; internal set; }

        [JsonProperty("systems")]
        public Next SolarSystems { get; private set; }

        [JsonProperty("time")]
        public Next Time { get; private set; }

        [JsonProperty("wars")]
        public Next Wars { get; private set; }
    }
}