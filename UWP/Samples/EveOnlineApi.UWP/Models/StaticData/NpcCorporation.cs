using Newtonsoft.Json;
using System.Diagnostics;

namespace EveOnlineApi.Models.StaticData
{
    [DebuggerDisplay("{Name}")]
    public sealed class NpcCorporation
    {
        [JsonProperty("description")]
        public string Description { get; internal set; }

        [JsonProperty("factionId")]
        public long FactionId { get; internal set; }

        [JsonProperty("iconId")]
        public long IconId { get; internal set; }

        [JsonProperty("id")]
        public long Id { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("scattered")]
        public bool Scattered { get; set; }

        [JsonProperty("solarSystemID")]
        public long SolarSystemId { get; internal set; }

        [JsonProperty("stationCount")]
        public int StationCount { get; set; }
    }
}