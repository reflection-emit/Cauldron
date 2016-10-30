using Newtonsoft.Json;
using System.Diagnostics;

namespace EveOnlineApi.Models.StaticData
{
    [DebuggerDisplay("{Name}")]
    public sealed class NpcStation
    {
        [JsonProperty("corporationID")]
        public long CorporationId { get; internal set; }

        [JsonProperty("id")]
        public long Id { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; internal set; }

        [JsonProperty("regionId")]
        public long RegionId { get; internal set; }

        [JsonProperty("reprocessingEfficiency")]
        public float ReprocessingEfficiency { get; internal set; }

        [JsonProperty("solarSystemID")]
        public long SolarSystemId { get; internal set; }

        [JsonProperty("stationTypeID")]
        public long StationTypeId { get; internal set; }
    }
}