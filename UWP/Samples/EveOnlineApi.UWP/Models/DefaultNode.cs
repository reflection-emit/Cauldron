using Newtonsoft.Json;
using System.Diagnostics;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("{Name}")]
    public class DefaultNode : Next
    {
        [JsonProperty("id")]
        public long Id { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; internal set; }
    }
}