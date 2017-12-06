using Newtonsoft.Json;
using System;

namespace EveOnlineApi.Models
{
    public class ItemAttributes : IKeyedModel
    {
        public ItemAttributes()
        {
            this.CachedUntil = DateTime.UtcNow.AddDays(1);
        }

        [JsonProperty("cachedUntil")]
        public DateTime CachedUntil { get; set; }

        [JsonProperty("capacity")]
        public double Capacity { get; internal set; }

        [JsonProperty("description")]
        public string Description { get; internal set; }

        [JsonProperty("iconID")]
        public long IconId { get; internal set; }

        [JsonProperty("id")]
        public long Id { get; internal set; }

        public string Key { get; set; }

        [JsonProperty("mass")]
        public double Mass { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; internal set; }

        [JsonProperty("portionSize")]
        public int PortionSize { get; internal set; }

        [JsonProperty("published")]
        public bool Published { get; internal set; }

        [JsonProperty("radius")]
        public double Radius { get; internal set; }

        [JsonProperty("volume")]
        public double Volume { get; internal set; }
    }
}