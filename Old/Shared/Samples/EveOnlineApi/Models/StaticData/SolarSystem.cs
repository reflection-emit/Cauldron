using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace EveOnlineApi.Models.StaticData
{
    [DebuggerDisplay("{Name}")]
    public class SolarSystem : DefaultNode, IEquatable<SolarSystem>
    {
        public bool Equals(SolarSystem other) =>
            other != null && this.Id == other.Id;
    }
}