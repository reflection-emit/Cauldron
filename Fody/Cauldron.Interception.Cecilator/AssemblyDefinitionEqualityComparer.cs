using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public int GetHashCode(AssemblyDefinition obj) => obj?.FullName.GetHashCode() ?? 0;
    }

    public class MethodInfoEqualityComparer : IEqualityComparer<MethodInfo>
    {
        public bool Equals(MethodInfo x, MethodInfo y) => x.Name == y.Name && y.GetParameters().SequenceEqual(x.GetParameters());

        public int GetHashCode(MethodInfo obj) => obj.Name.GetHashCode();
    }
}