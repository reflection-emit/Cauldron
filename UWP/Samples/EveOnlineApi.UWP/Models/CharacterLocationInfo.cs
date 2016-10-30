using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("{System.Name}")]
    public class CharacterLocationInfo : IEquatable<CharacterLocationInfo>
    {
        [JsonProperty("station")]
        public DefaultNode Station { get; internal set; }

        [JsonProperty("solarSystem")]
        public DefaultNode System { get; internal set; }

        public bool Equals(CharacterLocationInfo other) =>
            other != null && this.System.Id == other.System.Id;
    }
}