using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("{TypeID}, {Level}")]
    public sealed class SkillQueue
    {
        [XmlDeserializerAttribute("endTime")]
        [JsonProperty("endTime")]
        public DateTime EndTime { get; internal set; }

        [XmlDeserializerAttribute("level")]
        [JsonProperty("level")]
        public int Level { get; internal set; }

        [XmlDeserializerAttribute("queuePosition")]
        [JsonProperty("queuePosition")]
        public int QueuePosition { get; internal set; }

        [XmlDeserializerAttribute("startTime")]
        [JsonProperty("startTime")]
        public DateTime StartTime { get; internal set; }

        [XmlDeserializerAttribute("typeID")]
        [JsonProperty("typeID")]
        public long TypeID { get; internal set; }
    }
}