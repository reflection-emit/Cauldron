using Newtonsoft.Json;
using System;

namespace EveOnlineApi.Models
{
    public sealed class EveTime
    {
        [JsonProperty("time")]
        public DateTime Time { get; internal set; }
    }
}