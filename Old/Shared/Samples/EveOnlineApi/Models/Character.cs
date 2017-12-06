using Newtonsoft.Json;
using System.Diagnostics;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("{Name}, {CorporationName}")]
    public sealed class Character
    {
        [XmlDeserializerAttribute("allianceID")]
        [JsonProperty("allianceID")]
        public long AllianceId { get; internal set; }

        [XmlDeserializerAttribute("allianceName")]
        [JsonProperty("allianceName")]
        public string AllianceName { get; internal set; }

        [XmlDeserializerAttribute("characterID")]
        [JsonProperty("characterID")]
        public long CharacterId { get; internal set; }

        [XmlDeserializerAttribute("corporationID")]
        [JsonProperty("corporationID")]
        public long CorporationId { get; internal set; }

        [XmlDeserializerAttribute("corporationName")]
        [JsonProperty("corporationName")]
        public string CorporationName { get; internal set; }

        [XmlDeserializerAttribute("factionID")]
        [JsonProperty("factionID")]
        public long FactionId { get; internal set; }

        [XmlDeserializerAttribute("factionName")]
        [JsonProperty("factionName")]
        public string FactionName { get; internal set; }

        [XmlDeserializerAttribute("name")]
        [JsonProperty("name")]
        public string Name { get; internal set; }
    }
}