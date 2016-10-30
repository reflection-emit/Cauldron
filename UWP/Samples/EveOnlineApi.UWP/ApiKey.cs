using Newtonsoft.Json;
using System;

namespace EveOnlineApi
{
    public class ApiKey
    {
        public ApiKey(string keyId, string verificationCode)
        {
            this.KeyId = keyId;
            this.VerificationCode = verificationCode;
        }

        [JsonProperty("keyId")]
        public string KeyId { get; private set; }

        [JsonProperty("verificationCode")]
        public string VerificationCode { get; private set; }
    }

    public class CharacterSpecificApiKey : ApiKey
    {
        public CharacterSpecificApiKey(string keyId, string verificationCode, long characterId) : base(keyId, verificationCode)
        {
            this.CharacterId = characterId;
        }

        [JsonProperty("characterId")]
        public long CharacterId { get; private set; }
    }

    /// <summary>
    /// Register your application here https://developers.eveonline.com/applications
    /// </summary>
    public class CrestApiKey : ApiKey
    {
        public CrestApiKey(ApiKey apiKey, string clientId, string secretKey, string callbackUrl) : base(apiKey.KeyId, apiKey.VerificationCode)
        {
            if (apiKey == null)
                throw new ArgumentNullException(nameof(apiKey));

            if (clientId == null)
                throw new ArgumentNullException(nameof(clientId));

            if (secretKey == null)
                throw new ArgumentNullException(nameof(secretKey));

            if (callbackUrl == null)
                throw new ArgumentNullException(nameof(callbackUrl));

            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentException(nameof(clientId));

            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentException(nameof(secretKey));

            if (string.IsNullOrEmpty(callbackUrl))
                throw new ArgumentException(nameof(callbackUrl));

            this.ClientId = clientId;
            this.SecretKey = secretKey;
            this.CallbackUrl = callbackUrl;
        }

        [JsonProperty("callbackUrl")]
        public string CallbackUrl { get; private set; }

        [JsonProperty("clientId")]
        public string ClientId { get; private set; }

        [JsonProperty("secretKey")]
        public string SecretKey { get; private set; }
    }
}