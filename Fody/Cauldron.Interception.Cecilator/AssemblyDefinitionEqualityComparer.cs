using Mono.Cecil;
using System.Collections.Generic;

namespace Cauldron.Interception.Cecilator
{
    public sealed class AssemblyDefinitionEqualityComparer : IEqualityComparer<AssemblyDefinition>
    {
        public bool Equals(AssemblyDefinition x, AssemblyDefinition y)
        {
            if (object.ReferenceEquals(x, y))
                return true;

            if (object.Equals(x, null))
                return false;

            if (object.Equals(y, null))
                return false;

            if (y.FullName == x.FullName)
                return true;

            return false;
        }

        public int GetHashCode(AssemblyDefinition obj) => obj.GetHashCode();
    }
}