using Newtonsoft.Json;
using System.Diagnostics;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("{MessageID}")]
    public sealed class MailBody
    {
        [XmlDeserializerInnerText]
        [JsonProperty("content")]
        public string Content { get; internal set; }

        [XmlDeserializerAttribute("messageID")]
        [JsonProperty("messageId")]
        public long MessageID { get; internal set; }

        [JsonIgnore]
        public string PrettyfiedContent { get { return EveUtils.PrettifyMailBody(this.Content); } }
    }
}