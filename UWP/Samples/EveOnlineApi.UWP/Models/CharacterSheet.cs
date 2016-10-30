using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("{CharacterName}, {CorporationName}, {Bloodline}, {Race}, {IskBalance}")]
    public class CharacterSheet : CharacterSheetCompactWithSkills
    {
        private Location _homestation;

        [XmlDeserializerElement("allianceID"), XmlDeserializerNodePath("result")]
        [JsonProperty("allianceID")]
        public long AllianceId { get; internal set; }

        [XmlDeserializerElement("allianceName"), XmlDeserializerNodePath("result")]
        [JsonProperty("allianceName")]
        public string AllianceName { get; internal set; }

        [XmlDeserializerElement("ancestry"), XmlDeserializerNodePath("result")]
        [JsonProperty("ancestry")]
        public string Ancestry { get; internal set; }

        [XmlDeserializerElement("bloodLine"), XmlDeserializerNodePath("result")]
        [JsonProperty("bloodLine")]
        public string Bloodline { get; internal set; }

        [XmlDeserializerElement("charisma"), XmlDeserializerNodePath("result/attributes")]
        [JsonProperty("charisma")]
        public int Charisma { get; internal set; }

        [XmlDeserializerElement("cloneJumpDate"), XmlDeserializerNodePath("result")]
        [JsonProperty("cloneJumpDate")]
        public DateTime CloneJumpDate { get; internal set; }

        [XmlDeserializerNodePath("result/rowset", "name", "corporationTitles")]
        [XmlDeserializerElement("rowset", true)]
        [JsonProperty("corporationTitles")]
        public CorporationTitles CorporationTitles { get; internal set; }

        [XmlDeserializerElement("DoB"), XmlDeserializerNodePath("result")]
        [JsonProperty("DoB")]
        public DateTime DayOfBirth { get; internal set; }

        [XmlDeserializerElement("factionName"), XmlDeserializerNodePath("result")]
        [JsonProperty("factionName")]
        public string FactionName { get; internal set; }

        [XmlDeserializerElement("freeRespecs"), XmlDeserializerNodePath("result")]
        [JsonProperty("freeRespecs")]
        public int FreeRespec { get; internal set; }

        [XmlDeserializerElement("freeSkillPoints"), XmlDeserializerNodePath("result")]
        [JsonProperty("freeSkillPoints")]
        public long FreeSkillpoints { get; internal set; }

        [XmlDeserializerElement("gender"), XmlDeserializerNodePath("result")]
        [JsonProperty("gender")]
        public string Gender { get; internal set; }

        public Location Homestation
        {
            get
            {
                if (this._homestation == null)
                    this._homestation = new Location(this.HomestationId);

                return this._homestation;
            }
        }

        [XmlDeserializerElement("homeStationID"), XmlDeserializerNodePath("result")]
        [JsonProperty("homeStationID")]
        public long HomestationId { get; internal set; }

        [XmlDeserializerNodePath("result/rowset", "name", "implants")]
        [XmlDeserializerElement("rowset", true)]
        [JsonProperty("implants")]
        public List<Implant> Implants { get; internal set; }

        [XmlDeserializerElement("intelligence"), XmlDeserializerNodePath("result/attributes")]
        [JsonProperty("intelligence")]
        public int Intelligence { get; internal set; }

        [XmlDeserializerElement("balance"), XmlDeserializerNodePath("result")]
        [JsonProperty("balance")]
        public double IskBalance { get; internal set; }

        [XmlDeserializerElement("jumpActivation"), XmlDeserializerNodePath("result")]
        [JsonProperty("jumpActivation")]
        public DateTime JumpActivation { get; internal set; }

        [XmlDeserializerNodePath("result/rowset", "name", "jumpCloneImplants")]
        [XmlDeserializerElement("rowset", true)]
        [JsonProperty("jumpCloneImplants")]
        public List<JumpCloneImplant> JumpCloneImplant { get; internal set; }

        [XmlDeserializerNodePath("result/rowset", "name", "jumpClones")]
        [XmlDeserializerElement("rowset", true)]
        [JsonProperty("jumpClones")]
        public List<JumpClone> JumpClones { get; internal set; }

        [XmlDeserializerElement("jumpFatigue"), XmlDeserializerNodePath("result")]
        [JsonProperty("jumpFatigue")]
        public DateTime JumpFatigue { get; internal set; }

        [XmlDeserializerElement("jumpLastUpdate"), XmlDeserializerNodePath("result")]
        [JsonProperty("jumpLastUpdate")]
        public DateTime JumpLastUpdate { get; internal set; }

        [XmlDeserializerElement("lastRespecDate"), XmlDeserializerNodePath("result")]
        [JsonProperty("lastRespecDate")]
        public DateTime LastRespecDate { get; internal set; }

        [XmlDeserializerElement("lastTimedRespec"), XmlDeserializerNodePath("result")]
        [JsonProperty("lastTimedRespec")]
        public DateTime LastTimeRespec { get; internal set; }

        [XmlDeserializerElement("memory"), XmlDeserializerNodePath("result/attributes")]
        [JsonProperty("memory")]
        public int Memory { get; internal set; }

        [XmlDeserializerElement("perception"), XmlDeserializerNodePath("result/attributes")]
        [JsonProperty("perception")]
        public int Perception { get; internal set; }

        [XmlDeserializerElement("race"), XmlDeserializerNodePath("result")]
        [JsonProperty("race")]
        public string Race { get; internal set; }

        [XmlDeserializerElement("remoteStationDate"), XmlDeserializerNodePath("result")]
        [JsonProperty("remoteStationDate")]
        public DateTime RemoteStationDate { get; internal set; }

        [XmlDeserializerElement("willpower"), XmlDeserializerNodePath("result/attributes")]
        [JsonProperty("willpower")]
        public int Willpower { get; internal set; }
    }

    [DebuggerDisplay("{CharacterName}, {CorporationName}")]
    public class CharacterSheetCompact : IKeyedModel
    {
        [XmlDeserializerElement("cachedUntil")]
        [JsonProperty("cachedUntil")]
        public DateTime CachedUntil { get; set; }

        [XmlDeserializerElement("name"), XmlDeserializerNodePath("result")]
        [JsonProperty("name")]
        public string CharacterName { get; internal set; }

        [XmlDeserializerElement("corporationID"), XmlDeserializerNodePath("result")]
        [JsonProperty("corporationID")]
        public long CorporationId { get; internal set; }

        [XmlDeserializerElement("corporationName"), XmlDeserializerNodePath("result")]
        [JsonProperty("corporationName")]
        public string CorporationName { get; internal set; }

        [JsonProperty("key")]
        public string Key { get; set; }
    }

    [DebuggerDisplay("{Skills.Count}")]
    public class CharacterSheetCompactWithSkills : CharacterSheetCompact
    {
        [XmlDeserializerNodePath("result/rowset", "name", "skills")]
        [XmlDeserializerElement("rowset", true)]
        [JsonProperty("skills")]
        public List<Skill> Skills { get; internal set; }
    }
}