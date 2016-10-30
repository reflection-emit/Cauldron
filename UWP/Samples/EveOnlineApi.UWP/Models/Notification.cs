using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("{SenderName} - {NotificationTitle}")]
    public sealed class Notification
    {
        [XmlDeserializerAttribute("read")]
        [JsonProperty("read")]
        public bool IsRead { get; private set; }

        [XmlDeserializerAttribute("notificationID")]
        [JsonProperty("notificationID")]
        public long NotificationId { get; private set; }

        public string NotificationTitle { get { return NotificationTypes.GetString((int)this.TypeId); } }

        [XmlDeserializerAttribute("senderID")]
        [JsonProperty("senderID")]
        public long SenderId { get; private set; }

        [XmlDeserializerAttribute("senderName")]
        [JsonProperty("senderName")]
        public string SenderName { get; private set; }

        [XmlDeserializerAttribute("sentDate")]
        [JsonProperty("sentDate")]
        public DateTime SentDate { get; private set; }

        [XmlDeserializerAttribute("typeID")]
        [JsonProperty("typeID")]
        public long TypeId { get; private set; }
    }
}