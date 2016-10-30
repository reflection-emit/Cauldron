using Newtonsoft.Json;
using System.Diagnostics;

namespace EveOnlineApi.Models.StaticData
{
    [DebuggerDisplay("{Name}, {Description}")]
    public sealed class InventoryFlag
    {
        [JsonProperty("text")]
        public string Description { get; internal set; }

        [JsonProperty("id")]
        public int Id { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; internal set; }

        [JsonProperty("orderId")]
        public long OrderID { get; internal set; }
    }
}