using Mono.Cecil;
using System.Collections.Generic;

namespace Cauldron.Interception.Cecilator
{
    internal class TypeReferenceEqualityComparer : IEqualityComparer<TypeReference>
    {
        public bool Equals(TypeReference x, TypeReference y) => x.FullName == y.FullName;

        public int GetHashCode(TypeReference obj) => obj.FullName.GetHashCode();
    }
}