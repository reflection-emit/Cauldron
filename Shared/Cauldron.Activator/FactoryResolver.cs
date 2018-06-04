using Cauldron.Core.Reflection;
using System;
using System.Linq;
using System.Reflection;

namespace Cauldron.Activator
{
    /// <summary>
    /// Represents a collection of resolvers.
    /// </summary>
    public sealed class FactoryResolver
    {
        private FactoryStringDictionary<Func<IFactoryTypeInfo>> resolver = new FactoryStringDictionary<Func<IFactoryTypeInfo>>();

        /// <summary>
        /// Adds a new contractname resolver to the dictionary.
        /// </summary>
        /// <param name="contractName">The contractname to resolve.</param>
        /// <param name="factoryTypeInfo">
        /// The factory type info of the given contract name.
        /// This can be an item from <see cref="Factory.FactoryTypes"/> or a custom item.
        /// </param>
        public void Add(string contractName, IFactoryTypeInfo factoryTypeInfo) => this.resolver.Add(contractName, new Func<IFactoryTypeInfo>(() => factoryTypeInfo));

        /// <summary>
        /// Adds a new contractname resolver to the dictionary.
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from.</param>
        /// <param name="factoryTypeInfo">
        /// The factory type info of the given contract name.
        /// This can be an item from <see cref="Factory.FactoryTypes"/> or a custom item.
        /// </param>
        public void Add(Type contractType, IFactoryTypeInfo factoryTypeInfo) => this.resolver.Add(contractType.FullName, new Func<IFactoryTypeInfo>(() => factoryTypeInfo));

        /// <summary>
        /// Adds a new contractname resolver to the dictionary.
        /// </summary>
        /// <param name="contractName">The contractname to resolve.</param>
        /// <param name="resolveToTypeName">The name of the type that is assigned to the contractname.</param>
        public void Add(string contractName, string resolveToTypeName)
        {
            var type = Assemblies.GetTypeFromName(resolveToTypeName);
            if (type == null)
                throw new NullReferenceException($"Unable to find a type named '{resolveToTypeName}'");

            this.Add(contractName, type);
        }

        /// <summary>
        /// Adds a new contractname resolver to the dictionary.
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from.</param>
        /// <param name="resolveToTypeName">The name of the type that is assigned to the contractname.</param>
        public void Add(Type contractType, string resolveToTypeName) => this.Add(contractType.FullName, resolveToTypeName);

        /// <summary>
        /// Adds a new contractname resolver to the dictionary.
        /// </summary>
        /// <param name="contractName">The contractname to resolve.</param>
        /// <param name="func">A function to execute on ambiguous match for the given contract name.</param>
        public void Add(string contractName, Func<IFactoryTypeInfo> func) => this.resolver.Add(contractName, func);

        /// <summary>
        /// Adds a new contractname resolver to the dictionary.
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from.</param>
        /// <param name="func">A function to execute on ambiguous match for the given contract name.</param>
        public void Add(Type contractType, Func<IFactoryTypeInfo> func) => this.resolver.Add(contractType.FullName, func);

        /// <summary>
        /// Adds a new contractname resolver to the dictionary.
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from.</param>
        /// <param name="resolveToType">The type that is assigned to the contractname.</param>
        public void Add(Type contractType, Type resolveToType) => this.Add(contractType.FullName, resolveToType);

        /// <summary>
        /// Adds a new contractname resolver to the dictionary.
        /// </summary>
        /// <param name="contractName">The contractname to resolve.</param>
        /// <param name="resolveToType">The type that is assigned to the contractname.</param>
        public void Add(string contractName, Type resolveToType)
        {
            var factoryTypeInfo = Factory.FactoryTypes.FirstOrDefault(x => x.ContractName == contractName && x.Type == resolveToType);
            if (factoryTypeInfo == null)
                throw new NullReferenceException($"Unable to find the type '{resolveToType}' with the contractname '{contractName}'. Make sure that the type is decorated with the {nameof(ComponentAttribute)}.");

            this.resolver.Add(contractName, new Func<IFactoryTypeInfo>(() => factoryTypeInfo));
        }

        internal IFactoryTypeInfo SelectAmbiguousMatch(string contractName)
        {
            var factoryTypeInfo = resolver[contractName];

            if (factoryTypeInfo == null)
                throw new AmbiguousMatchException(
                    string.Join(@"\r\n\", new string[] {
                    $"Unable to resolve the contract '{contractName}'.",
                    "The Factory has found multiple implementations, but it is not defined which one to use.",
                    "To define, use Factory.Resolver.Add",
                    }));

            return factoryTypeInfo();
        }
    }
}