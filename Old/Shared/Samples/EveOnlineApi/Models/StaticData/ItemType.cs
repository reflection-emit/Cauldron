using Newtonsoft.Json;
using System.Diagnostics;

namespace EveOnlineApi.Models.StaticData
{
    [DebuggerDisplay("{Name}, {GroupName}")]
    public sealed class ItemType
    {
        private ItemGroup _group;
        private MarketGroup _marketGroup;

        [JsonProperty("capacity")]
        public float Capacity { get; internal set; }

        [JsonProperty("description")]
        public string Description { get; internal set; }

        [JsonIgnore]
        public ItemGroup Group
        {
            get
            {
                if (this._group == null)
                    this._group = Api.Current.StaticData.GetGroup(this.GroupId);

                return this._group;
            }
        }

        [JsonProperty("groupID")]
        public long GroupId { get; internal set; }

        [JsonIgnore]
        public string GroupName { get { return this.Group == null ? string.Empty : this.Group.Name; } }

        [JsonProperty("id")]
        public long Id { get; internal set; }

        [JsonProperty("published")]
        public bool IsPublished { get; internal set; }

        [JsonIgnore]
        public MarketGroup MarketGroup
        {
            get
            {
                if (this._marketGroup == null)
                    this._marketGroup = Api.Current.StaticData.GetMarketGroup(this.MarketGroupId);

                return this._marketGroup;
            }
        }

        [JsonProperty("marketGroupID")]
        public long MarketGroupId { get; internal set; }

        [JsonProperty("mass")]
        public float Mass { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; internal set; }

        [JsonProperty("portionSize")]
        public int PortionSize { get; internal set; }

        [JsonProperty("volume")]
        public float Volume { get; internal set; }
    }
}