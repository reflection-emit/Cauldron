using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("{SenderName}, {Title}")]
    public sealed class MailMessageItem
    {
        [XmlDeserializerAttribute("messageID")]
        [JsonProperty("messageID")]
        public int MessageId { get; internal set; }

        [XmlDeserializerAttribute("sentDate")]
        [JsonProperty("sentDate")]
        public DateTime SendDate { get; internal set; }

        [XmlDeserializerAttribute("senderID")]
        [JsonProperty("senderID")]
        public int SenderId { get; internal set; }

        [XmlDeserializerAttribute("senderName")]
        [JsonProperty("senderName")]
        public string SenderName { get; internal set; }

        [XmlDeserializerAttribute("title")]
        [JsonProperty("title")]
        public string Title { get; internal set; }

        [XmlDeserializerAttribute("toCharacterIDs")]
        [JsonProperty("toCharacterIDs")]
        public long[] ToCharacterIds { get; internal set; }

        [XmlDeserializerAttribute("toCorpOrAllianceID")]
        [JsonProperty("toCorpOrAllianceID")]
        public long[] ToCorpOrAllianceId { get; internal set; }

        [XmlDeserializerAttribute("toListID")]
        [JsonProperty("toListID")]
        public long[] ToListId { get; internal set; }
    }
}