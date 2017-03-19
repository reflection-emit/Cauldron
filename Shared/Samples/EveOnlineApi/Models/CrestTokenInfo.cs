using Newtonsoft.Json;
using System;

namespace EveOnlineApi.Models
{
    public sealed class CrestTokenInfo
    {
        public CrestTokenInfo()
        {
            this.CreationDate = DateTime.Now;
        }

        [JsonProperty("access_token")]
        public string AccessToken { get; internal set; }

        [JsonProperty("authentification")]
        public string Authentification { get; internal set; }

        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; internal set; }

        [JsonIgnore]
        public DateTime ExpirationDate { get { return this.CreationDate.AddSeconds(this.ExpiresIn); } }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; internal set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; internal set; }

        [JsonProperty("token_type")]
        public string TokenType { get; internal set; }
    }
}