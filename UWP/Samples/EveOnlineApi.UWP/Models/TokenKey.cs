using Newtonsoft.Json;
using System;
using System.Linq;

namespace EveOnlineApi.Models
{
    public sealed class TokenKey : IEquatable<TokenKey>
    {
        [JsonProperty("accessScope")]
        public string[] AccessScope { get; internal set; }

        [JsonProperty("id")]
        public long CharacterId { get; internal set; }

        [JsonProperty("serverName")]
        public string ServerName { get; internal set; }

        [JsonProperty("tokenInfo")]
        public CrestTokenInfo TokenInfo { get; internal set; }

        public bool Equals(TokenKey other) =>
            other != null &&
            this.CharacterId == other.CharacterId &&
            AccessScope.SequenceEqual(other.AccessScope);
    }
}