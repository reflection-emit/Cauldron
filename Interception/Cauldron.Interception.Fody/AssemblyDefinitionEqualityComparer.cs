using Mono.Cecil;
using System.Collections.Generic;

namespace Cauldron.Interception.Fody
{
    internal class AssemblyDefinitionEqualityComparer : IEqualityComparer<AssemblyDefinition>
    {
        public bool Equals(AssemblyDefinition x, AssemblyDefinition y) => x.FullName == y.FullName;

        public int GetHashCode(AssemblyDefinition obj) => obj.FullName.GetHashCode();
    }
}