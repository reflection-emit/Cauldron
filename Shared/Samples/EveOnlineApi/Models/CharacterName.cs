using Newtonsoft.Json;

namespace EveOnlineApi.Models
{
    public sealed class CharacterName
    {
        [XmlDeserializerAttribute("characterID")]
        [JsonProperty("characterID")]
        public long CharacterId { get; set; }

        [XmlDeserializerAttribute("name")]
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}