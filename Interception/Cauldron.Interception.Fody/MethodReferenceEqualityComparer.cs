using Mono.Cecil;
using System.Collections.Generic;

namespace Cauldron.Interception.Fody
{
    public class MethodReferenceEqualityComparer : IEqualityComparer<MethodReference>
    {
        public bool Equals(MethodReference x, MethodReference y) => x.FullName == y.FullName;

        public int GetHashCode(MethodReference obj) => obj.FullName.GetHashCode();
    }
}