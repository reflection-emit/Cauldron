using Newtonsoft.Json;
using System;

namespace EveOnlineApi.Models
{
    public sealed class PriceHistory
    {
        [JsonProperty("avgPrice")]
        public double AveragePrice { get; internal set; }

        [JsonProperty("date")]
        public DateTime Date { get; internal set; }

        [JsonProperty("highPrice")]
        public double HighPrice { get; internal set; }

        [JsonProperty("lowPrice")]
        public double LowPrice { get; internal set; }

        [JsonProperty("orderCount")]
        public long OrderCount { get; internal set; }

        [JsonProperty("volume")]
        public long Volume { get; internal set; }
    }
}