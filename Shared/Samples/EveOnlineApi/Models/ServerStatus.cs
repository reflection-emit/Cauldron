using Newtonsoft.Json;

namespace EveOnlineApi.Models
{
    public sealed class ServerStatus
    {
        [XmlDeserializerElement("onlinePlayers")]
        [XmlDeserializerNodePath("result")]
        [JsonProperty("onlinePlayers")]
        public long OnlinePlayers { get; internal set; }

        [XmlDeserializerElement("serverOpen")]
        [XmlDeserializerNodePath("result")]
        [JsonProperty("serverOpen")]
        public bool ServerOpen { get; internal set; }
    }
}