using Mono.Cecil;
using System.Collections.Generic;

namespace Cauldron.Interception.Cecilator
{
    internal class TypeReferenceEqualityComparer : IEqualityComparer<TypeReference>
    {
        public bool Equals(TypeReference x, TypeReference y) => x == y;

        public int GetHashCode(TypeReference obj) => obj.GetHashCode();
    }
}