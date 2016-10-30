using Newtonsoft.Json;
using System.Diagnostics;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("{Type.Id}")]
    public sealed class OpenMarketBody
    {
        [JsonProperty("type")]
        public OpenMarketBodyType Type { get; internal set; }
    }

    [DebuggerDisplay("{HRef}")]
    public sealed class OpenMarketBodyType : Next
    {
        [JsonProperty("id")]
        public long Id { get; internal set; }
    }
}