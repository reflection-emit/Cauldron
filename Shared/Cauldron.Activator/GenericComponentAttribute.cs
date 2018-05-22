using System;

namespace Cauldron.Activator
{
    [AttributeUsage(AttributeTargets.Module, AllowMultiple = true, Inherited = false)]
    public sealed class GenericComponentAttribute : Attribute
    {
        public GenericComponentAttribute(Type contractType, Type type, FactoryCreationPolicy policy, uint priority)
            : this(contractType.FullName, type, policy, priority)
        {
        }

        public GenericComponentAttribute(Type contractType, Type type)
            : this(contractType.FullName, type, FactoryCreationPolicy.Instanced, 0)
        {
        }

        public GenericComponentAttribute(Type contractType, Type type, FactoryCreationPolicy policy)
            : this(contractType.FullName, type, policy, 0)
        {
        }

        public GenericComponentAttribute(string contractName, Type type, FactoryCreationPolicy policy)
            : this(contractName, type, policy, 0)
        {
        }

        public GenericComponentAttribute(string contractName, Type type)
            : this(contractName, type, FactoryCreationPolicy.Instanced, 0)
        {
        }

        public GenericComponentAttribute(string contractName, Type type, FactoryCreationPolicy policy, uint priority)
        {
        }
    }
}