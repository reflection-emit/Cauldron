using Newtonsoft.Json;
using System.Collections.Generic;

namespace EveOnlineApi.Models
{
    public abstract class ModelCollection<T>
    {
        [JsonProperty("items")]
        public IEnumerable<T> Items { get; internal set; }

        [JsonProperty("next")]
        public Next Next { get; internal set; }

        [JsonProperty("pageCount")]
        public int PageCount { get; internal set; }

        [JsonProperty("totalCount")]
        public uint TotalCount { get; internal set; }
    }
}