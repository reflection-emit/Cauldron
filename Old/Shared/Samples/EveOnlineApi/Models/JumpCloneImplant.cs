using Newtonsoft.Json;
using System.Diagnostics;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("{Name}")]
    public sealed class JumpCloneImplant
    {
        [JsonProperty("id")]
        public string Id
        {
            get { return this.Key + this.JumpCloneId.ToString() + this.TypeId.ToString(); }
            set { /* Just dummy for the serializer */}
        }

        [XmlDeserializerAttribute("jumpCloneID")]
        [JsonProperty("jumpCloneID")]
        public long JumpCloneId { get; internal set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [XmlDeserializerAttribute("typeName")]
        [JsonProperty("typeName")]
        public string Name { get; internal set; }

        [XmlDeserializerAttribute("typeID")]
        [JsonProperty("typeID")]
        public long TypeId { get; internal set; }
    }
}