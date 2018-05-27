using System;

namespace Cauldron.Activator
{
    /// <summary>
    /// Specifies that the decorated <see cref="Type"/> is a dependency injection component.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ComponentAttribute : Attribute
    {
        /// <summary>
        /// Initializes an instance of <see cref="ComponentAttribute"/>
        /// </summary>
        /// <param name="contractName">The contract name that is used to export the type</param>
        /// <param name="policy">The creation policy</param>
        /// <param name="priority">The priority of the component. The <see cref="Factory"/> uses this information to sort the instances for <see cref="Factory.CreateMany(string, object[])"/></param>. 0 is lowest; uint.Max is highest.
        /// <exception cref="ArgumentNullException">Parameter <paramref name="contractName"/> is null</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="contractName"/> is empty</exception>
        public ComponentAttribute(string contractName, FactoryCreationPolicy policy, uint priority)
        {
            if (contractName == null)
                throw new ArgumentNullException(nameof(contractName));

            if (contractName.Length == 0)
                throw new ArgumentException("The parameter is an empty string", nameof(contractName));

            this.Policy = policy;
            this.ContractName = contractName;
            this.Priority = priority;
        }

        /// <summary>
        /// Initializes an instance of <see cref="ComponentAttribute"/>
        /// </summary>
        /// <param name="contractName">The contract name that is used to export the type</param>
        /// <param name="priority">The priority of the component. The <see cref="Factory"/> uses this information to sort the instances for <see cref="Factory.CreateMany(string, object[])"/></param>. 0 is lowest; uint.Max is highest.
        /// <exception cref="ArgumentNullException">Parameter <paramref name="contractName"/> is null</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="contractName"/> is empty</exception>
        public ComponentAttribute(string contractName, uint priority) : this(contractName, FactoryCreationPolicy.Instanced, priority)
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="ComponentAttribute"/>. The default <see cref="ComponentAttribute.Priority"/> is 0.
        /// </summary>
        /// <param name="contractName">The contract name that is used to export the type</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="contractName"/> is null</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="contractName"/> is empty</exception>
        public ComponentAttribute(string contractName) : this(contractName, FactoryCreationPolicy.Instanced, 0)
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="ComponentAttribute"/>
        /// </summary>
        /// <param name="contractType">A type from which to derive the contract name that is used to export the type</param>
        /// <param name="priority">The priority of the component. The <see cref="Factory"/> uses this information to sort the instances for <see cref="Factory.CreateMany(string, object[])"/></param>. 0 is lowest; uint.Max is highest.
        public ComponentAttribute(Type contractType, uint priority) : this(contractType.FullName, FactoryCreationPolicy.Instanced, priority)
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="ComponentAttribute"/>. The default <see cref="ComponentAttribute.Priority"/> is 0.
        /// </summary>
        /// <param name="contractType">A type from which to derive the contract name that is used to export the type</param>
        public ComponentAttribute(Type contractType) : this(contractType.FullName, FactoryCreationPolicy.Instanced, 0)
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="ComponentAttribute"/>
        /// </summary>
        /// <param name="contractType">A type from which to derive the contract name that is used to export the type</param>
        /// <param name="policy">The creation policy</param>
        /// <param name="priority">The priority of the component. The <see cref="Factory"/> uses this information to sort the instances for <see cref="Factory.CreateMany(string, object[])"/></param>. 0 is lowest; uint.Max is highest.
        public ComponentAttribute(Type contractType, FactoryCreationPolicy policy, uint priority) : this(contractType.FullName, policy, priority)
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="ComponentAttribute"/>. The default <see cref="ComponentAttribute.Priority"/> is 0.
        /// </summary>
        /// <param name="contractType">A type from which to derive the contract name that is used to export the type</param>
        /// <param name="policy">The creation policy</param>
        public ComponentAttribute(Type contractType, FactoryCreationPolicy policy) : this(contractType.FullName, policy, 0)
        {
        }

        /// <summary>
        /// Gets the contract name of the object export
        /// </summary>
        public string ContractName { get; private set; }

        /// <summary>
        /// Gets or sets a value that tells the weaver to implement the event invokation of <see cref="Factory.ObjectCreated"/>.
        /// </summary>
        public bool InvokeOnObjectCreationEvent { get; set; } = false;

        /// <summary>
        /// Gets the creation policy
        /// </summary>
        public FactoryCreationPolicy Policy { get; private set; }

        /// <summary>
        /// Gets the priority of the component
        /// </summary>
        public uint Priority { get; private set; }
    }
}