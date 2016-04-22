using System;
using System.Reflection;

namespace Cauldron
{
    internal struct FactoryTypeInfo
    {
        public string contractName;
        public FactoryCreationPolicy creationPolicy;
        public Type type;
        public TypeInfo typeInfo;
    }
}