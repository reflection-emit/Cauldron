using Newtonsoft.Json;

namespace EveOnlineApi.Models
{
    public sealed class ItemProperties
    {
        [JsonProperty("description")]
        public string Description { get; internal set; }

        [JsonProperty("iconId")]
        public long IconId { get; internal set; }

        [JsonProperty("mass")]
        public double Mass { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; internal set; }

        [JsonProperty("volume")]
        public double Volume { get; internal set; }
    }
}