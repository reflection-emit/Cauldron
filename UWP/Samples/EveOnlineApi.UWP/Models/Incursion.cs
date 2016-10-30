using EveOnlineApi.Models.StaticData;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("{IncursionType}, {AggressorFaction.Name}")]
    public sealed class Incursion
    {
        [JsonProperty("aggressorFactionID")]
        public Faction AggressorFaction { get; internal set; }

        [JsonProperty("hasBoss")]
        public bool HasBoss { get; internal set; }

        [JsonProperty("incursionType")]
        public string IncursionType { get; internal set; }

        [JsonProperty("infestedSolarSystems")]
        public IEnumerable<SolarSystem> InfestedSolarSystems { get; internal set; }

        [JsonProperty("influence")]
        public float Influence { get; internal set; }

        [JsonProperty("stagingSolarSystem")]
        public SolarSystem StagingSolarSystem { get; internal set; }

        [JsonProperty("state")]
        public string State { get; internal set; }
    }

    [DebuggerDisplay("Count = {Count}")]
    public sealed class IncursionCollection : ModelCollection<Incursion>
    {
    }
}