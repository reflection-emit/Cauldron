using System;

namespace Cauldron.Activator
{
    [AttributeUsage(AttributeTargets.Module | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class GenericComponentAttribute : ComponentAttribute
    {
        public GenericComponentAttribute(Type type, Type contractType, FactoryCreationPolicy policy, uint priority)
            : this(type, contractType.FullName, policy, priority)
        {
        }

        public GenericComponentAttribute(Type type, Type contractType)
            : this(type, contractType.FullName, FactoryCreationPolicy.Instanced, 0)
        {
        }

        public GenericComponentAttribute(Type type, Type contractType, FactoryCreationPolicy policy)
            : this(type, contractType.FullName, policy, 0)
        {
        }

        public GenericComponentAttribute(Type type, string contractName, FactoryCreationPolicy policy)
            : this(type, contractName, policy, 0)
        {
        }

        public GenericComponentAttribute(Type type, string contractName)
            : this(type, contractName, FactoryCreationPolicy.Instanced, 0)
        {
        }

        public GenericComponentAttribute(Type type, string contractName, FactoryCreationPolicy policy, uint priority) : base(contractName, policy, priority)
        {
        }
    }
}