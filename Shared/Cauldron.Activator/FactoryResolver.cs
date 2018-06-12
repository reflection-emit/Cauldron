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
        private FactoryStringDictionary<Func<Type, IFactoryTypeInfo[], IFactoryTypeInfo>> resolverNamed = new FactoryStringDictionary<Func<Type, IFactoryTypeInfo[], IFactoryTypeInfo>>();
        private FactoryDictionary<Type, Func<Type, IFactoryTypeInfo[], IFactoryTypeInfo>> resolverTypes = new FactoryDictionary<Type, Func<Type, IFactoryTypeInfo[], IFactoryTypeInfo>>();

        /// <summary>
        /// Adds a new contractname resolver to the dictionary.
        /// </summary>
        /// <param name="contractName">The contractname to resolve.</param>
        /// <param name="factoryTypeInfo">
        /// The factory type info of the given contract name.
        /// This can be an item from <see cref="Factory.FactoryTypes"/> or a custom item.
        /// </param>
        public void Add(string contractName, IFactoryTypeInfo factoryTypeInfo) =>
            this.resolverNamed.Add(contractName, new Func<Type, IFactoryTypeInfo[], IFactoryTypeInfo>((callingType, types) => factoryTypeInfo));

        /// <summary>
        /// Adds a new contractname resolver to the dictionary.
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from.</param>
        /// <param name="factoryTypeInfo">
        /// The factory type info of the given contract name.
        /// This can be an item from <see cref="Factory.FactoryTypes"/> or a custom item.
        /// </param>
        public void Add(Type contractType, IFactoryTypeInfo factoryTypeInfo) =>
            this.AddInternal(contractType, new Func<Type, IFactoryTypeInfo[], IFactoryTypeInfo>((callingType, types) => factoryTypeInfo));

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
        public void Add(Type contractType, string resolveToTypeName)
        {
            var type = Assemblies.GetTypeFromName(resolveToTypeName);
            if (type == null)
                throw new NullReferenceException($"Unable to find a type named '{resolveToTypeName}'");

            this.Add(contractType, type);
        }

        /// <summary>
        /// Adds a new contractname resolver to the dictionary.
        /// </summary>
        /// <param name="contractName">The contractname to resolve.</param>
        /// <param name="func">A function to execute on ambiguous match for the given contract name.</param>
        public void Add(string contractName, Func<Type, IFactoryTypeInfo[], IFactoryTypeInfo> func) => this.resolverNamed.Add(contractName, func);

        /// <summary>
        /// Adds a new contractname resolver to the dictionary.
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from.</param>
        /// <param name="func">A function to execute on ambiguous match for the given contract name.</param>
        public void Add(Type contractType, Func<Type, IFactoryTypeInfo[], IFactoryTypeInfo> func) => this.AddInternal(contractType, func);

        /// <summary>
        /// Adds a new contractname resolver to the dictionary.
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from.</param>
        /// <param name="resolveToType">The type that is assigned to the contractname.</param>
        public void Add(Type contractType, Type resolveToType)
        {
            var factoryTypeInfo = Factory.FactoryTypes.FirstOrDefault(x => x.ContractType == contractType && x.Type == resolveToType);
            if (factoryTypeInfo == null)
                throw new NullReferenceException($"Unable to find the type '{resolveToType}' with the contractname '{contractType}'. Make sure that the type is decorated with the {nameof(ComponentAttribute)}.");

            this.AddInternal(contractType, new Func<Type, IFactoryTypeInfo[], IFactoryTypeInfo>((callingType, types) => factoryTypeInfo));
        }

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

            this.resolverNamed.Add(contractName, new Func<Type, IFactoryTypeInfo[], IFactoryTypeInfo>((callingType, types) => factoryTypeInfo));
        }

        internal IFactoryTypeInfo SelectAmbiguousMatch(Type callingType, Type contractType, IFactoryTypeInfo[] ambigiousTypes)
        {
            var factoryTypeInfo = resolverTypes[contractType];

            if (factoryTypeInfo == null)
                throw new AmbiguousMatchException(
                    string.Join(@"\r\n\", new string[] {
                    $"Unable to resolve the contract '{contractType}'.",
                    "The Factory has found multiple implementations, but it is not defined which one to use.",
                    "To define, use Factory.Resolver.Add",
                    }));

            return factoryTypeInfo(callingType, ambigiousTypes);
        }

        internal IFactoryTypeInfo SelectAmbiguousMatch(Type callingType, string contractName, IFactoryTypeInfo[] ambigiousTypes)
        {
            var factoryTypeInfo = resolverNamed[contractName];

            if (factoryTypeInfo == null)
                throw new AmbiguousMatchException(
                    string.Join(@"\r\n\", new string[] {
                    $"Unable to resolve the contract '{contractName}'.",
                    "The Factory has found multiple implementations, but it is not defined which one to use.",
                    "To define, use Factory.Resolver.Add",
                    }));

            return factoryTypeInfo(callingType, ambigiousTypes);
        }

        private void AddInternal(Type contractType, Func<Type, IFactoryTypeInfo[], IFactoryTypeInfo> func)
        {
            this.resolverTypes.Add(contractType, new Func<Type, IFactoryTypeInfo[], IFactoryTypeInfo>(func));
            this.resolverNamed.Add(contractType.FullName, new Func<Type, IFactoryTypeInfo[], IFactoryTypeInfo>(func));
        }
    }
}