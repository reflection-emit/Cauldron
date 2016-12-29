using Cauldron.Core.Extensions;
using System.Collections.Generic;
using System.Diagnostics;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("Count = {Count}")]
    public sealed class CorporationTitles : List<CorporationTitle>
    {
        public override string ToString() => this.Join(", ");
    }
}