using Newtonsoft.Json;
using System.Diagnostics;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("{Name}")]
    public sealed class JumpClone
    {
        private Location _location;

        [JsonProperty("id")]
        public string Id
        {
            get { return this.Key + this.JumpCloneId.ToString(); }
            set { /* Just dummy for the serializer */}
        }

        [XmlDeserializerAttribute("jumpCloneID")]
        [JsonProperty("jumpCloneID")]
        public long JumpCloneId { get; internal set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonIgnore]
        public Location Location
        {
            get
            {
                if (this._location == null)
                    this._location = new Location(this.LocationId);

                return this._location;
            }
        }

        [XmlDeserializerAttribute("locationID")]
        [JsonProperty("locationID")]
        public long LocationId { get; internal set; }

        [XmlDeserializerAttribute("cloneName")]
        [JsonProperty("cloneName")]
        public string Name { get; internal set; }

        [XmlDeserializerAttribute("typeID")]
        [JsonProperty("typeID")]
        public long TypeId { get; internal set; }
    }
}