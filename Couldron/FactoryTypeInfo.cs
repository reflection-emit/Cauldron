using System;
using System.Reflection;

namespace Couldron
{
    internal struct FactoryTypeInfo
    {
        public ConstructorInfo constructor;
        public string contractName;
        public FactoryCreationPolicy creationPolicy;
        public Type type;
        public TypeInfo typeInfo;
    }
}