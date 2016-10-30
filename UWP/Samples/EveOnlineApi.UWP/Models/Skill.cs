using Newtonsoft.Json;

namespace EveOnlineApi.Models
{
    public sealed class Skill
    {
        [JsonProperty("id")]
        public string Id
        {
            get { return this.Key + this.TypeId.ToString(); }
            set { /* Just dummy for the serializer */}
        }

        [JsonProperty("key")]
        public string Key { get; set; }

        [XmlDeserializerAttribute("level")]
        [JsonProperty("level")]
        public byte SkillLevel { get; internal set; }

        [XmlDeserializerAttribute("skillpoints")]
        [JsonProperty("skillpoints")]
        public long Skillpoints { get; internal set; }

        [XmlDeserializerAttribute("typeID")]
        [JsonProperty("typeID")]
        public long TypeId { get; internal set; }
    }
}