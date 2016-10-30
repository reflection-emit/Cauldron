using System;
using System.Reflection;

namespace Cauldron.Activator
{
    /// <summary>
    /// Holds data that is required by the <see cref="Factory"/> to create instances
    /// </summary>
    public sealed class FactoryTypeInfo
    {
        /// <summary>
        /// The contract name associated with the type
        /// </summary>
        public readonly string contractName;

        /// <summary>
        /// The creation policy of the type
        /// </summary>
        public readonly FactoryCreationPolicy creationPolicy;

        /// <summary>
        /// The constructor information used to construct the type. This value can be null if the <see cref="Factory"/> uses a method (<see cref="FactoryTypeInfo.objectConstructorMethodInfo"/>)
        /// to construct the type or the <see cref="Factory"/> is needs to auto-detect the constructor.
        /// </summary>
        public readonly ConstructorInfo objectConstructorInfo;

        /// <summary>
        /// The method information of the method used to construct the type. This value can be null. If both <see cref="FactoryTypeInfo.objectConstructorInfo"/> and
        /// <see cref="FactoryTypeInfo.objectConstructorMethodInfo"/> is not null, then this value will be ignored.
        /// </summary>
        public readonly MethodInfo objectConstructorMethodInfo;

        /// <summary>
        /// The type to be constructed
        /// </summary>
        public readonly Type type;

        /// <summary>
        /// THe <see cref="TypeInfo"/> of the type
        /// </summary>
        public readonly TypeInfo typeInfo;

        internal FactoryTypeInfo(
            string contractName,
            FactoryCreationPolicy creationPolicy,
            ConstructorInfo objectConstructorInfo,
            MethodInfo objectConstructorMethodInfo,
            Type type,
            TypeInfo typeInfo)
        {
            this.contractName = contractName;
            this.creationPolicy = creationPolicy;
            this.objectConstructorInfo = objectConstructorInfo;
            this.objectConstructorMethodInfo = objectConstructorMethodInfo;
            this.type = type;
            this.typeInfo = typeInfo;
        }
    }
}