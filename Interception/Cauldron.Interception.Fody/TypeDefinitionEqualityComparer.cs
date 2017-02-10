using Mono.Cecil;
using System.Collections.Generic;

namespace Cauldron.Interception.Fody
{
    public class TypeDefinitionEqualityComparer : IEqualityComparer<TypeDefinition>
    {
        public bool Equals(TypeDefinition x, TypeDefinition y) => x.FullName == y.FullName;

        public int GetHashCode(TypeDefinition obj) => obj.FullName.GetHashCode();
    }
}