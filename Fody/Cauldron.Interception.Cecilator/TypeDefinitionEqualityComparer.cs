using Mono.Cecil;
using System.Collections.Generic;

namespace Cauldron.Interception.Cecilator
{
    internal class TypeDefinitionEqualityComparer : IEqualityComparer<TypeDefinition>
    {
        public bool Equals(TypeDefinition x, TypeDefinition y) =>
            x.Module.Assembly.FullName.GetHashCode() == y.Module.Assembly.FullName.GetHashCode() &&
            x.GetHashCode() == y.GetHashCode() &&
            x.FullName.GetHashCode() == y.FullName.GetHashCode() &&
            x.FullName == y.FullName;

        public int GetHashCode(TypeDefinition obj) =>
            obj.Module.Assembly.FullName.GetHashCode() ^
            obj.GetHashCode();
    }
}