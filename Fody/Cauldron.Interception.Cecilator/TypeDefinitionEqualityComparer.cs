using Mono.Cecil;
using System.Collections.Generic;

namespace Cauldron.Interception.Cecilator
{
    internal class TypeDefinitionEqualityComparer : IEqualityComparer<TypeDefinition>
    {
        public bool Equals(TypeDefinition x, TypeDefinition y) => x.AreEqual(y);

        public int GetHashCode(TypeDefinition obj) => obj.Module.GetHashCode() ^ obj.GetHashCode();
    }
}