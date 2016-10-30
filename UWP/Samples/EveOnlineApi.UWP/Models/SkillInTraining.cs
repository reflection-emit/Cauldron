using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("TrainingTypeID = {TrainingTypeID}, Level = {TrainingToLevel}")]
    public sealed class SkillInTraining : IKeyedModel
    {
        [XmlDeserializerElement("cachedUntil")]
        [JsonProperty("cachedUntil")]
        public DateTime CachedUntil { get; set; }

        [XmlDeserializerElement("skillInTraining"), XmlDeserializerNodePath("result")]
        [JsonProperty("skillInTraining")]
        public bool IsInTraining { get; internal set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [XmlDeserializerElement("trainingEndTime"), XmlDeserializerNodePath("result")]
        [JsonProperty("trainingEndTime")]
        public DateTime TrainingEndTime { get; internal set; }

        [XmlDeserializerElement("trainingToLevel"), XmlDeserializerNodePath("result")]
        [JsonProperty("trainingToLevel")]
        public int TrainingToLevel { get; internal set; }

        [XmlDeserializerElement("trainingTypeID"), XmlDeserializerNodePath("result")]
        [JsonProperty("trainingTypeID")]
        public long TrainingTypeID { get; internal set; }
    }
}