using Newtonsoft.Json;

namespace EveOnlineApi.Models
{
    public sealed class ConquerableStation
    {
        [XmlDeserializerAttribute("corporationID")]
        [JsonProperty("corporationID")]
        public long CorporationID { get; internal set; }

        [XmlDeserializerAttribute("corporationName")]
        [JsonProperty("corporationName")]
        public string CorporationName { get; internal set; }

        [XmlDeserializerAttribute("stationID")]
        [JsonProperty("stationID")]
        public long Id { get; internal set; }

        [XmlDeserializerAttribute("stationName")]
        [JsonProperty("stationName")]
        public string Name { get; internal set; }

        [XmlDeserializerAttribute("solarSystemID")]
        [JsonProperty("solarSystemID")]
        public long SolarSystemID { get; internal set; }

        [XmlDeserializerAttribute("stationTypeID")]
        [JsonProperty("stationTypeID")]
        public long StationTypeID { get; internal set; }
    }
}