using Newtonsoft.Json;

namespace EveOnlineApi.Models
{
    public class Next
    {
        [JsonProperty("href")]
        public string HRef { get; internal set; }
    }
}