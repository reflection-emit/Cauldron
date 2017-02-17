using System.Collections.Generic;

namespace Cauldron.Interception.Cecilator
{
    internal class BuilderTypeEqualityComparer : IEqualityComparer<BuilderType>
    {
        public bool Equals(BuilderType x, BuilderType y) => x.typeReference.FullName == y.typeReference.FullName;

        public int GetHashCode(BuilderType obj) => obj.typeReference.FullName.GetHashCode();
    }
}