using Mono.Cecil;
using System.Collections.Generic;

namespace Cauldron.Interception.Cecilator
{
    internal class CustomAttributeEqualityComparer : IEqualityComparer<CustomAttribute>
    {
        public bool Equals(CustomAttribute x, CustomAttribute y) => x == y;

        public int GetHashCode(CustomAttribute obj) => obj.GetHashCode();
    }
}