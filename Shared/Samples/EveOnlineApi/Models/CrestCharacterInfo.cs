using Newtonsoft.Json;
using System;

namespace EveOnlineApi.Models
{
    public sealed class CrestCharacterInfo : IEquatable<CrestCharacterInfo>, IEquatable<long>
    {
        [JsonProperty("CharacterID")]
        public long CharacterId { get; private set; }

        [JsonProperty("CharacterName")]
        public string CharacterName { get; private set; }

        public bool Equals(CrestCharacterInfo other) =>
            other != null && other.CharacterId == this.CharacterId;

        public bool Equals(long other) => this.CharacterId == other;
    }
}