using System;

namespace Cauldron.Activator
{
    /// <summary>
    /// Specifies that the described generic <see cref="Type"/> is a dependency injection component.
    /// </summary>
    [AttributeUsage(AttributeTargets.Module, AllowMultiple = true, Inherited = false)]
    public sealed class GenericComponentAttribute : ComponentAttribute
    {
        /// <summary>
        /// Initializes an instance of <see cref="GenericComponentAttribute"/>
        /// </summary>
        /// <param name="type">The generic type to be defined as component.</param>
        /// <param name="contractType">A type from which to derive the contract name that is used to export the type</param>
        /// <param name="policy">The creation policy</param>
        /// <param name="priority">The priority of the component. The <see cref="Factory"/> uses this information to sort the instances for <see cref="Factory.CreateMany(string, object[])"/></param>. 0 is lowest; uint.Max is highest.
        public GenericComponentAttribute(Type type, Type contractType, FactoryCreationPolicy policy, uint priority)
            : this(type, contractType.FullName, policy, priority)
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="GenericComponentAttribute"/>
        /// </summary>
        /// <param name="type">The generic type to be defined as component.</param>
        /// <param name="contractType">A type from which to derive the contract name that is used to export the type</param>
        public GenericComponentAttribute(Type type, Type contractType)
            : this(type, contractType.FullName, FactoryCreationPolicy.Instanced, 0)
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="GenericComponentAttribute"/>
        /// </summary>
        /// <param name="type">The generic type to be defined as component.</param>
        /// <param name="contractType">A type from which to derive the contract name that is used to export the type</param>
        /// <param name="policy">The creation policy</param>
        public GenericComponentAttribute(Type type, Type contractType, FactoryCreationPolicy policy)
            : this(type, contractType.FullName, policy, 0)
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="GenericComponentAttribute"/>
        /// </summary>
        /// <param name="type">The generic type to be defined as component.</param>
        /// <param name="contractName">The contract name that is used to export the type</param>
        /// <param name="policy">The creation policy</param>
        public GenericComponentAttribute(Type type, string contractName, FactoryCreationPolicy policy)
            : this(type, contractName, policy, 0)
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="GenericComponentAttribute"/>
        /// </summary>
        /// <param name="type">The generic type to be defined as component.</param>
        /// <param name="contractName">The contract name that is used to export the type</param>
        public GenericComponentAttribute(Type type, string contractName)
            : this(type, contractName, FactoryCreationPolicy.Instanced, 0)
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="GenericComponentAttribute"/>
        /// </summary>
        /// <param name="type">The generic type to be defined as component.</param>
        /// <param name="contractName">The contract name that is used to export the type</param>
        /// <param name="policy">The creation policy</param>
        /// <param name="priority">The priority of the component. The <see cref="Factory"/> uses this information to sort the instances for <see cref="Factory.CreateMany(string, object[])"/></param>. 0 is lowest; uint.Max is highest.
        public GenericComponentAttribute(Type type, string contractName, FactoryCreationPolicy policy, uint priority) : base(contractName, policy, priority)
        {
        }
    }
}