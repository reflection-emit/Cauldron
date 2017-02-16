using Mono.Cecil;
using System.Collections.Generic;

namespace Cauldron.Interception.Cecilator
{
    internal class AssemblyDefinitionEqualityComparer : IEqualityComparer<AssemblyDefinition>
    {
        public bool Equals(AssemblyDefinition x, AssemblyDefinition y) => x == y;

        public int GetHashCode(AssemblyDefinition obj) => obj.GetHashCode();
    }
}