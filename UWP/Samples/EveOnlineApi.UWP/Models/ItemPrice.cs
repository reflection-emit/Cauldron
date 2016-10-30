using Cauldron.Activator;
using EveOnlineApi.Models.StaticData;
using Newtonsoft.Json;

namespace EveOnlineApi.Models
{
    public sealed class ItemId
    {
        [JsonProperty("id")]
        public long Id { get; internal set; }
    }

    public sealed class ItemPrice
    {
        private ItemType _type;

        [JsonProperty("adjustedPrice")]
        public double AdjustedPrice { get; internal set; }

        [JsonProperty("averagePrice")]
        public double AveragePrice { get; internal set; }

        [JsonProperty("type")]
        public ItemId ItemId { get; internal set; }

        [JsonIgnore]
        public ItemType Type
        {
            get
            {
                if (this._type == null)
                    this._type = Api.Current.StaticData.GetItemType(this.ItemId.Id);

                return this._type;
            }
        }
    }
}