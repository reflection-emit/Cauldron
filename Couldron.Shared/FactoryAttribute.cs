using System;

namespace Couldron
{
    /// <summary>
    /// Specifies that <see cref="Type"/> provide a particular export
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class FactoryAttribute : Attribute
    {
        /// <summary>
        /// Initializes an instance of <see cref="FactoryAttribute"/>
        /// </summary>
        /// <param name="contractName">The contract name that is used to export the type</param>
        /// <param name="policy">The creation policy</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="contractName"/> is null</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="contractName"/> is empty</exception>
        public FactoryAttribute(string contractName, FactoryCreationPolicy policy)
        {
            if (contractName == null)
                throw new ArgumentNullException(nameof(contractName));

            if (contractName.Length == 0)
                throw new ArgumentException("The parameter is an empty string", nameof(contractName));

            this.Policy = policy;
            this.ContractName = contractName;
        }

        /// <summary>
        /// Initializes an instance of <see cref="FactoryAttribute"/>
        /// </summary>
        /// <param name="contractName">The contract name that is used to export the type</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="contractName"/> is null</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="contractName"/> is empty</exception>
        public FactoryAttribute(string contractName) : this(contractName, FactoryCreationPolicy.Instanced)
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="FactoryAttribute"/>
        /// </summary>
        /// <param name="contractType">A type from which to derive the contract name that is used to export the type</param>
        public FactoryAttribute(Type contractType) : this(contractType.FullName, FactoryCreationPolicy.Instanced)
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="FactoryAttribute"/>
        /// </summary>
        /// <param name="contractType">A type from which to derive the contract name that is used to export the type</param>
        /// <param name="policy">The creation policy</param>
        public FactoryAttribute(Type contractType, FactoryCreationPolicy policy) : this(contractType.FullName, policy)
        {
        }

        /// <summary>
        /// Gets the contract name of the object export
        /// </summary>
        public string ContractName { get; private set; }

        /// <summary>
        /// Gets the creation policy
        /// </summary>
        public FactoryCreationPolicy Policy { get; private set; }
    }
}