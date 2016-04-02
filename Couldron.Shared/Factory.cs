using Couldron.Collections;
using Couldron.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Couldron
{
    /// <summary>
    /// Provides methods for creating and destroying object instances
    /// </summary>
    public static class Factory
    {
        private static List<IFactoryExtension> factoryExtensions = new List<IFactoryExtension>();
        private static ConcurrentList<ObjectKey> instances = new ConcurrentList<ObjectKey>();
        private static FactoryTypeInfo[] types;

        static Factory()
        {
            // Get all factory extensions
            factoryExtensions.AddRange(AssemblyUtil.GetTypesImplementsInterface<IFactoryExtension>()
                .Select(x => Activator.CreateInstance(x.AsType()) as IFactoryExtension));

            var attributeType = typeof(FactoryAttribute);
            var definedTypes = AssemblyUtil.ExportedTypes
                .Where(x => !x.IsInterface)
                .Select(x => new
                {
                    Attrib = x.GetCustomAttribute<FactoryAttribute>(false),
                    TypeInfo = x
                });

            var types = new List<FactoryTypeInfo>();

            foreach (var d in definedTypes)
            {
                var type = d.TypeInfo.AsType();

                for (int i = 0; i < factoryExtensions.Count; i++)
                    factoryExtensions[i].OnInitialize(d.TypeInfo, type);

                var contractName = d.Attrib == null ? type.FullName : d.Attrib.ContractName;
                var policy = d.Attrib == null ? FactoryCreationPolicy.Instanced : d.Attrib.Policy;

                types.Add(new FactoryTypeInfo
                {
                    contractName = contractName,
                    creationPolicy = policy,
                    type = type,
                    typeInfo = d.TypeInfo
                });
            }

            Factory.types = types.ToArray();
        }

        /// <summary>
        /// Creates an instance of the specified type depending on the <see cref="FactoryAttribute"/> and <see cref="InjectionConstructorAttribute"/>.
        /// </summary>
        /// <typeparam name="T">The Type that contract name derives from</typeparam>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of
        /// the constructor to invoke. If args is an empty array or null, the constructor
        /// that takes no parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A reference to the newly created object.</returns>
        /// <exception cref="KeyNotFoundException">The contract described by <typeparamref name="T"/> was not found</exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="AmbiguousMatchException">There is more than one implementation with contractname <typeparamref name="T"/> found.</exception>
        /// <exception cref="NotSupportedException">An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an implemented <see cref="IDisposable"/> must also implement the <see cref="IDisposableObject"/> interface.</exception>
        public static T Create<T>(params object[] parameters)
        {
            return (T)GetInstance(typeof(T).FullName, parameters);
        }

        /// <summary>
        /// Creates an instance of the specified type depending on the <see cref="FactoryAttribute"/> and <see cref="InjectionConstructorAttribute"/>.
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of
        /// the constructor to invoke. If args is an empty array or null, the constructor
        /// that takes no parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A reference to the newly created object.</returns>
        /// <exception cref="ArgumentNullException">The parameter <paramref name="contractName"/> is null</exception>
        /// <exception cref="ArgumentException">The parameter <paramref name="contractName"/> is an empty string</exception>
        /// <exception cref="KeyNotFoundException">The contract described by <paramref name="contractName"/> was not found</exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="AmbiguousMatchException">There is more than one implementation with contractname <paramref name="contractName"/> found.</exception>
        /// <exception cref="NotSupportedException">An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an implemented <see cref="IDisposable"/> must also implement the <see cref="IDisposableObject"/> interface.</exception>
        public static object Create(string contractName, params object[] parameters)
        {
            if (contractName == null)
                throw new ArgumentNullException(nameof(contractName));

            if (contractName.Length == 0)
                throw new ArgumentException("The parameter is an empty string", nameof(contractName));

            return GetInstance(contractName, parameters);
        }

        /// <summary>
        /// Creates an instance of the specified type depending on the <see cref="FactoryAttribute"/> and <see cref="InjectionConstructorAttribute"/>.
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from</param>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of
        /// the constructor to invoke. If args is an empty array or null, the constructor
        /// that takes no parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A reference to the newly created object.</returns>
        /// <exception cref="ArgumentNullException">The parameter <paramref name="contractType"/> is null</exception>
        /// <exception cref="KeyNotFoundException">The contract described by <paramref name="contractType"/> was not found</exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="AmbiguousMatchException">There is more than one implementation with contractname <paramref name="contractType"/> found.</exception>
        /// <exception cref="NotSupportedException">An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an implemented <see cref="IDisposable"/> must also implement the <see cref="IDisposableObject"/> interface.</exception>
        public static object Create(Type contractType, params object[] parameters)
        {
            if (contractType == null)
                throw new ArgumentNullException(nameof(contractType));

            return GetInstance(contractType.FullName, parameters);
        }

        /// <summary>
        /// Creates instances of the specified type depending on the <see cref="FactoryAttribute"/> and <see cref="InjectionConstructorAttribute"/>.
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of
        /// the constructor to invoke. If args is an empty array or null, the constructor
        /// that takes no parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A collection of the newly created objects.</returns>
        /// <exception cref="ArgumentNullException">The parameter <paramref name="contractName"/> is null</exception>
        /// <exception cref="ArgumentException">The parameter <paramref name="contractName"/> is an empty string</exception>
        /// <exception cref="KeyNotFoundException">The contract described by <paramref name="contractName"/> was not found</exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="NotSupportedException">An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an implemented <see cref="IDisposable"/> must also implement the <see cref="IDisposableObject"/> interface.</exception>
        public static IEnumerable CreateMany(string contractName, params object[] parameters)
        {
            if (contractName == null)
                throw new ArgumentNullException(nameof(contractName));

            if (contractName.Length == 0)
                throw new ArgumentException("The parameter is an empty string", nameof(contractName));

            return GetInstances(contractName, parameters);
        }

        /// <summary>
        /// Creates instances of the specified type depending on the <see cref="FactoryAttribute"/> and <see cref="InjectionConstructorAttribute"/>.
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from</param>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of
        /// the constructor to invoke. If args is an empty array or null, the constructor
        /// that takes no parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A collection of the newly created objects.</returns>
        /// <exception cref="ArgumentNullException">The parameter <paramref name="contractType"/> is null</exception>
        /// <exception cref="KeyNotFoundException">The contract described by <paramref name="contractType"/> was not found</exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="NotSupportedException">An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an implemented <see cref="IDisposable"/> must also implement the <see cref="IDisposableObject"/> interface.</exception>
        public static IEnumerable CreateMany(Type contractType, params object[] parameters)
        {
            if (contractType == null)
                throw new ArgumentNullException(nameof(contractType));

            return GetInstances(contractType.FullName, parameters);
        }

        /// <summary>
        /// Creates instances of the specified type depending on the <see cref="FactoryAttribute"/> and <see cref="InjectionConstructorAttribute"/>.
        /// </summary>
        /// <typeparam name="T">The Type that contract name derives from</typeparam>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of
        /// the constructor to invoke. If args is an empty array or null, the constructor
        /// that takes no parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A collection of the newly created objects.</returns>
        /// <exception cref="KeyNotFoundException">The contract described by <typeparamref name="T"/> was not found</exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="NotSupportedException">An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an implemented <see cref="IDisposable"/> must also implement the <see cref="IDisposableObject"/> interface.</exception>
        public static IEnumerable<T> CreateMany<T>(params object[] parameters)
        {
            return GetInstances(typeof(T).FullName, parameters).Cast<T>();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <typeparam name="T">The Type that the contract name derives from</typeparam>
        public static void Destroy<T>()
        {
            Destroy(typeof(T));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="contractType">The Type that the contract name derives from</param>
        public static void Destroy(Type contractType)
        {
            Destroy(contractType.FullName);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        public static void Destroy(string contractName)
        {
            if (instances.Contains(x => x.FactoryTypeInfo.contractName == contractName))
                instances.Remove(x => x.FactoryTypeInfo.contractName == contractName).DisposeAll();
        }

        /// <summary>
        /// Determines whether a contract exist
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        /// <returns>True if the contract exists, otherwise false</returns>
        public static bool HasContract(string contractName)
        {
            return types.Contains(x => x.contractName == contractName);
        }

        /// <summary>
        /// Determines whether a contract exist
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from</param>
        /// <returns>True if the contract exists, otherwise false</returns>
        public static bool HasContract(Type contractType)
        {
            return HasContract(contractType.FullName);
        }

        /// <summary>
        /// Returns a value that indicates if the contract is a singleton or not
        /// </summary>
        /// <typeparam name="T">The type that the contract name derives from</typeparam>
        /// <returns>Returns null if the <typeparamref name="T"/> does not exist</returns>
        public static bool? IsSingleton<T>()
        {
            return IsSingleton(typeof(T).FullName);
        }

        /// <summary>
        /// Returns a value that indicates if the contract is a singleton or not
        /// </summary>
        /// <param name="contractType">The type that the contract name derives from</param>
        /// <returns>Returns null if the <paramref name="contractType"/> does not exist</returns>
        /// <exception cref="ArgumentNullException">The parameter <paramref name="contractType"/> is null</exception>
        public static bool? IsSingleton(Type contractType)
        {
            if (contractType == null)
                throw new ArgumentNullException(nameof(contractType));

            return IsSingleton(contractType.FullName);
        }

        /// <summary>
        /// Returns a value that indicates if the contract is a singleton or not
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        /// <returns>Returns null if the <paramref name="contractName"/> does not exist</returns>
        /// <exception cref="ArgumentNullException">The parameter <paramref name="contractName"/> is null</exception>
        /// <exception cref="ArgumentException">The <paramref name="contractName"/> is empty</exception>
        public static bool? IsSingleton(string contractName)
        {
            if (contractName == null)
                throw new ArgumentNullException(nameof(contractName));

            if (contractName.Length == 0)
                throw new ArgumentException("The parameter is an empty string", nameof(contractName));

            if (types.Contains(x => x.contractName == contractName))
                return types.First(x => x.contractName == contractName).creationPolicy == FactoryCreationPolicy.Singleton;

            return null;
        }

        private static object CreateObject(Type type, TypeInfo typeInfo, object[] parameterOverride)
        {
            object result = null;

            // Check if any extension can create the object
            var creatorExtension = factoryExtensions.FirstOrDefault(x => x.CanConstruct(type, typeInfo));

            // check if type has importing constructor
            if (creatorExtension != null)
                result = creatorExtension.Construct(type, typeInfo);
            else if (parameterOverride.Length > 0)
                result = Activator.CreateInstance(type, parameterOverride);
            else
                result = Activator.CreateInstance(type);

            // Activate all Extensions
            for (int i = 0; i < factoryExtensions.Count; i++)
                factoryExtensions[i].OnCreateObject(result, type, typeInfo);

            return result;
        }

        private static object GetInstance(FactoryTypeInfo factoryTypeInfo, object[] parameters)
        {
            if (factoryTypeInfo.creationPolicy == FactoryCreationPolicy.Instanced)
                return CreateObject(factoryTypeInfo.type, factoryTypeInfo.typeInfo, parameters);
            else if (factoryTypeInfo.creationPolicy == FactoryCreationPolicy.Singleton)
            {
                if (instances.Contains(x => x.FactoryTypeInfo.contractName == factoryTypeInfo.contractName))
                    return instances.First(x => x.FactoryTypeInfo.contractName == factoryTypeInfo.contractName).Item;
                else
                {
                    // Create the instance and return the object
                    var instance = CreateObject(factoryTypeInfo.type, factoryTypeInfo.typeInfo, parameters);

                    // every singleton that implements the idisposable interface has also to implement the IdisposableObject interface
                    // this is because we want to know if an instance was disposed (somehow)
                    var disposable = instance as IDisposable;
                    if (disposable != null)
                    {
                        var disposableObject = instance as IDisposableObject;
                        if (disposableObject == null)
                            throw new NotSupportedException("An object with creation policy 'Singleton' with an implemented 'IDisposable' must also implement the 'IDisposableObject' interface.");

                        disposableObject.Disposed += (s, e) => instances.Remove(x => x.Item == instance);
                    }

                    instances.Add(new ObjectKey { FactoryTypeInfo = factoryTypeInfo, Item = instance });

                    return instance;
                }
            }
            else
                throw new Exception("Unknown creation policy");
        }

        private static object GetInstance(string contractName, object[] parameters)
        {
            if (!types.Contains(x => x.contractName == contractName) && !instances.Contains(x => x.FactoryTypeInfo.contractName == contractName))
            {
                // Try to find out the type
                var realType = AssemblyUtil.GetTypeFromName(contractName);

                if (realType == null)
                    throw new KeyNotFoundException("The contractName '" + contractName + "' was not found.");

                return CreateObject(realType, realType.GetTypeInfo(), parameters);
            }

            var factoryTypeInfos = types.Where(x => x.contractName == contractName);

            if (factoryTypeInfos.Count() > 1)
                throw new AmbiguousMatchException("There is more than one implementation with contractname '" + contractName + "' found.");

            return GetInstance(factoryTypeInfos.First(), parameters);
        }

        private static IEnumerable GetInstances(string contractName, object[] parameters)
        {
            if (!types.Contains(x => x.contractName == contractName) && !instances.Contains(x => x.FactoryTypeInfo.contractName == contractName))
                throw new KeyNotFoundException("The contractName '" + contractName + "' was not found.");

            var factoryTypeInfos = types.Where(x => x.contractName == contractName);
            var result = new List<object>();

            foreach (var factoryTypeInfo in factoryTypeInfos)
                result.Add(GetInstance(factoryTypeInfo, parameters));

            return result;
        }

        private class ObjectKey
        {
            public FactoryTypeInfo FactoryTypeInfo { get; set; }
            public object Item { get; set; }

            public override string ToString()
            {
                return this.FactoryTypeInfo.contractName + " -> " + this.FactoryTypeInfo.type.FullName;
            }
        }
    }
}