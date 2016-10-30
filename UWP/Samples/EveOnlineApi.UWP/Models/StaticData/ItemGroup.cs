using Newtonsoft.Json;
using System.Diagnostics;

namespace EveOnlineApi.Models.StaticData
{
    [DebuggerDisplay("{Name}")]
    public sealed class ItemGroup
    {
        [JsonProperty("categoryID")]
        public long CategoryID { get; internal set; }

        [JsonProperty("id")]
        public long Id { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; internal set; }

        [JsonProperty("published")]
        public bool Published { get; internal set; }
    }
}