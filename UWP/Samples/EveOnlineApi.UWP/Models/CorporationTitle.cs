using Newtonsoft.Json;
using System.Diagnostics;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("{TitleName}")]
    public sealed class CorporationTitle
    {
        [JsonProperty("id")]
        public string Id
        {
            get { return this.Key + this.TitleId.ToString(); }
            set { /* Just dummy for the sqlite serializer */}
        }

        [JsonProperty("key")]
        public string Key { get; set; }

        [XmlDeserializerAttribute("titleID")]
        [JsonProperty("titleID")]
        public long TitleId { get; internal set; }

        [XmlDeserializerAttribute("titleName")]
        [JsonProperty("titleName")]
        public string TitleName { get; internal set; }

        public override string ToString()
        {
            return this.TitleName;
        }
    }
}