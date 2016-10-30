using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("{FlagDescription}, {Type.Name}")]
    public sealed class FittedItem
    {
        [JsonProperty("flag")]
        public int Flag { get; internal set; }

        [JsonIgnore]
        public string FlagDescription { get { return Api.Current.StaticData.GetFlagDescription(this.Flag); } }

        [JsonProperty("quantity")]
        public long Quantity { get; internal set; }

        [JsonProperty("type")]
        public DefaultNode Type { get; internal set; }
    }

    [DebuggerDisplay("{Name}")]
    public class ShipFitting
    {
        [JsonProperty("description")]
        public string Description { get; internal set; }

        [JsonProperty("fittingID")]
        public long Id { get; internal set; }

        [JsonProperty("items")]
        public IEnumerable<FittedItem> Items { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; internal set; }

        [JsonProperty("ship")]
        public DefaultNode Ship { get; internal set; }
    }

    public class ShipFittingCollection : ModelCollection<ShipFitting>
    {
    }
}