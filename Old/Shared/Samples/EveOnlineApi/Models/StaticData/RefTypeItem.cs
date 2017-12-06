using Newtonsoft.Json;

namespace EveOnlineApi.Models.StaticData
{
    public sealed class RefTypeItem
    {
        [XmlDeserializerAttribute("refTypeID")]
        [JsonProperty("id")]
        public long TypeId { get; internal set; }

        [XmlDeserializerAttribute("refTypeName")]
        [JsonProperty("name")]
        public string TypeName { get; internal set; }
    }
}