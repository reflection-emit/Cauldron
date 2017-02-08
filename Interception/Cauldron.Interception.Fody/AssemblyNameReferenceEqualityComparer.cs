using Mono.Cecil;
using System.Collections.Generic;

namespace Cauldron.Interception.Fody
{
    internal class AssemblyNameReferenceEqualityComparer : IEqualityComparer<AssemblyNameReference>
    {
        public bool Equals(AssemblyNameReference x, AssemblyNameReference y) => x.FullName == y.FullName;

        public int GetHashCode(AssemblyNameReference obj) => obj.FullName.GetHashCode();
    }
}