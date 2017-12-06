using EveOnlineApi.Models.StaticData;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EveOnlineApi.Models
{
    public sealed class SolarSystemInfo : SolarSystem
    {
        [JsonProperty("securityClass")]
        public string SecurityClass { get; internal set; }

        [JsonProperty("securityStatus")]
        public double SecurityStatus { get; internal set; }

        [JsonProperty("sovereignty")]
        public Corporation Sovereignty { get; internal set; }

        [JsonProperty("stargates")]
        public IEnumerable<DefaultNode> Stargates { get; internal set; }
    }
}