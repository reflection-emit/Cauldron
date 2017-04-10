using Cauldron.Core;
using Cauldron.Core.Collections;
using Cauldron.Core.Extensions;
using Cauldron.Internal;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

#if NETFX_CORE || DESKTOP

#elif NETCORE

using System.Threading;

#endif

namespace Cauldron.Activator
{
    /// <summary>
    /// Provides methods for creating and destroying object instances
    /// </summary>
    public sealed class Factory
    {
        private static IFactoryTypeInfo[] components;
        private static IFactoryExtension[] factoryExtensions;
        private static readonly string iFactoryExtensionName = typeof(IFactoryExtension).FullName;
        private static ConcurrentDictionary<string, FactoryInstancedObject> instances = new ConcurrentDictionary<string, FactoryInstancedObject>();

        static Factory()
        {
            // Get all known components
            components = Assemblies.CauldronObjects.Cast<IFactoryCache>().Where(x => x != null).SelectMany(x => x.GetComponents()).ToArray();
            // Get all factory extensions
            factoryExtensions = components
                .Where(x => x.ContractName.GetHashCode() == iFactoryExtensionName.GetHashCode() && x.ContractName == iFactoryExtensionName).Select(x => x.CreateInstance() as IFactoryExtension)
                .ToArray();

            Assemblies.LoadedAssemblyChanged += (s, e) =>
            {
                if (e.Cauldron == null)
                    return;

                // Get the factory cache
                var cache = e.Cauldron as IFactoryCache;
                if (cache != null)
                {
                    // Get all known components
                    components = components.Concat(cache.GetComponents());
                    // Get all factory extensions
                    factoryExtensions = factoryExtensions.Concat(cache.GetComponents()
                        .Where(x => x.ContractName.GetHashCode() == iFactoryExtensionName.GetHashCode() && x.ContractName == iFactoryExtensionName).Select(x => x.CreateInstance() as IFactoryExtension)
                        .ToArray());
                }
            };
        }

        /// <summary>
        /// Gets or sets a value that indicates if the <see cref="Factory"/> is allowed to raise an exception or not.
        /// </summary>
        public static bool CanRaiseExceptions { get; set; } = false;

        /// <summary>
        /// Gets a collection types that is known to the <see cref="Factory"/>
        /// </summary>
        public static IEnumerable<IFactoryTypeInfo> RegisteredTypes { get { return components.AsParallel(); } }

        /// <summary>
        /// Adds a new <see cref="Type"/> to list of known types.
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        /// <param name="creationPolicy">The creation policy of the type as defined by <see cref="FactoryCreationPolicy"/></param>
        /// <param name="type">The type to be added</param>
        /// <param name="createInstance">An action that is called by the factory to create the object</param>
        public static IFactoryTypeInfo AddType(string contractName, FactoryCreationPolicy creationPolicy, Type type, Func<object[], object> createInstance)
        {
            var factoryTypeInfo = new FactoryTypeInfoInternal(contractName, creationPolicy, type, createInstance);
            components = components.Concat(factoryTypeInfo);
            return factoryTypeInfo;
        }

        /// <summary>
        /// Creates an instance of the specified type depending on the <see cref="ComponentAttribute"/>
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
        public static T Create<T>(params object[] parameters) where T : class => GetInstance(typeof(T).FullName, parameters) as T;

        /// <summary>
        /// Creates an instance of the specified type depending on the <see cref="ComponentAttribute"/>
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
        /// Creates an instance of the specified type depending on the <see cref="ComponentAttribute"/>
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
        /// Creates an instance of the specified type using the constructor that best matches the specified parameters.
        /// This method is similar to <see cref="ExtensionsReflection.CreateInstance(Type, object[])"/>, but this takes the types defined with <see cref="ComponentAttribute"/> into
        /// account. This also executes the factory extensions (<see cref="IFactoryExtension"/>).
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        /// <param name="args">
        /// An array of arguments that match in number, order, and type the parameters of
        /// the constructor to invoke. If args is an empty array or null, the constructor
        /// that takes no parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A reference to the newly created object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null</exception>
        /// <exception cref="NotImplementedException">Implementation of <paramref name="type"/> not found</exception>
        public static object CreateInstance(Type type, params object[] args)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var factoryType = GetFactoryTypeInfoByType(type);
            object result = null;

            try
            {
                if (factoryType != null)
                    result = factoryType.CreateInstance(args);
                else
                    result = type.CreateInstance(args);
            }
            catch (TypeIsInterfaceException e)
            {
                throw new NotImplementedException($"Unable to find the implementation of '{type.FullName}'. Make sure that the Assembly with implementation was loaded to the AppDomain.", e);
            }
            catch
            {
                throw;
            }

            if (result != null)
            {
                // Activate all Extensions
                var c = factoryExtensions;
                for (int i = 0; i < c.Length; i++)
                    c[i].OnCreateObject(result, type);

                // Invoke the IFactoryInitializeComponent.OnInitializeComponent method if implemented
                result.As<IFactoryInitializeComponent>()?.OnInitializeComponent();
            }

            return result;
        }

        /// <summary>
        /// Creates instances of the specified type depending on the <see cref="ComponentAttribute"/>
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
        /// Creates instances of the specified type depending on the <see cref="ComponentAttribute"/>
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
        /// Creates instances of the specified type depending on the <see cref="ComponentAttribute"/>
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
        public static IEnumerable<T> CreateMany<T>(params object[] parameters) => GetInstances(typeof(T).FullName, parameters).Cast<T>();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <typeparam name="T">The Type that the contract name derives from</typeparam>
        public static void Destroy<T>() => Destroy(typeof(T));

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="contractType">The Type that the contract name derives from</param>
        public static void Destroy(Type contractType) => Destroy(contractType.FullName);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        public static void Destroy(string contractName)
        {
            foreach (var item in GetFactoryTypeInfos(contractName))
            {
                var key = contractName + item.Type.FullName;
                FactoryInstancedObject instance;

                if (instances.ContainsKey(key) && instances.TryRemove(key, out instance))
                    instance?.Dispose();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public static void Destroy()
        {
            var oldInstances = instances.ToArray();
            instances.Clear();

            oldInstances.TryDispose();
        }

        /// <summary>
        /// Determines whether a contract exist
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        /// <returns>True if the contract exists, otherwise false</returns>
        public static bool HasContract(string contractName) => GetFactoryTypeInfos(contractName).Any();

        /// <summary>
        /// Determines whether a contract exist
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from</param>
        /// <returns>True if the contract exists, otherwise false</returns>
        public static bool HasContract(Type contractType) => HasContract(contractType.FullName);

        /// <summary>
        /// Returns a value that indicates if the contract is a singleton or not
        /// </summary>
        /// <typeparam name="T">The type that the contract name derives from</typeparam>
        /// <returns>Returns null if the <typeparamref name="T"/> does not exist</returns>
        public static bool? IsSingleton<T>() => IsSingleton(typeof(T).FullName);

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

            var component = components.FirstOrDefault(x => x.ContractName.GetHashCode() == contractName.GetHashCode() && x.ContractName == contractName);

            if (component != null)
                return component.CreationPolicy == FactoryCreationPolicy.Singleton;

            return null;
        }

        /// <summary>
        /// Removes a <see cref="Type"/> from the list of known types
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        /// <param name="type">The type to be removed</param>
        public static void RemoveType(string contractName, Type type)
        {
            var tempArray = components.Copy();

            foreach (var factoryInfo in GetFactoryTypeInfos(contractName).Where(x => x.Type == type))
            {
                for (int i = 0; i < tempArray.Length; i++)
                    if (tempArray[i].ContractName.GetHashCode() == factoryInfo.ContractName.GetHashCode() && tempArray[i].ContractName == factoryInfo.ContractName && factoryInfo.Type == type)
                        tempArray[i] = null;
            }

            components = tempArray.RemoveNull();
        }

        private static object CreateInstance(IFactoryTypeInfo factoryType, params object[] args)
        {
            if (factoryType == null)
                throw new ArgumentNullException(nameof(factoryType));

            var result = factoryType.CreateInstance(args);

            if (result != null)
            {
                // Activate all Extensions
                var c = factoryExtensions;
                for (int i = 0; i < c.Length; i++)
                    c[i].OnCreateObject(result, factoryType.Type);

                // Invoke the IFactoryInitializeComponent.OnInitializeComponent method if implemented
                result.As<IFactoryInitializeComponent>()?.OnInitializeComponent();
            }

            return result;
        }

        private static object GetInstance(IFactoryTypeInfo factoryTypeInfo, object[] parameters)
        {
            if (factoryTypeInfo.CreationPolicy == FactoryCreationPolicy.Instanced)
                return CreateInstance(factoryTypeInfo, parameters);
            else if (factoryTypeInfo.CreationPolicy == FactoryCreationPolicy.Singleton)
            {
                FactoryInstancedObject existingInstance;

                if (instances.TryGetValue(factoryTypeInfo.ContractName + factoryTypeInfo.Type.FullName, out existingInstance))
                    return existingInstance.Item;
                else
                {
                    // Create the instance and return the object
                    var newInstance = CreateInstance(factoryTypeInfo, parameters);
                    var key = factoryTypeInfo.ContractName + factoryTypeInfo.Type.FullName;

                    // every singleton that implements the idisposable interface has also to implement the IdisposableObject interface
                    // this is because we want to know if an instance was disposed (somehow)
                    var disposable = newInstance as IDisposable;
                    if (disposable != null)
                    {
                        var disposableObject = newInstance as IDisposableObject;
                        if (disposableObject == null)
                            throw new NotSupportedException("An object with creation policy 'Singleton' with an implemented 'IDisposable' must also implement the 'IDisposableObject' interface.");

                        disposableObject.Disposed += (s, e) =>
                        {
                            FactoryInstancedObject thisInstance;

                            if (instances.ContainsKey(key) && instances.TryRemove(key, out thisInstance))
                                thisInstance?.Dispose();
                        };
                    }

                    instances.TryAdd(key, new FactoryInstancedObject { FactoryTypeInfo = factoryTypeInfo, Item = newInstance });

                    return newInstance;
                }
            }
            else
                throw new Exception("Unknown creation policy");
        }

        private static IFactoryTypeInfo GetFactoryTypeInfoByType(Type type)
        {
            var c = components;
            for (int i = 0; i < c.Length; i++)
                if (c[i].Type.GetHashCode() == type.GetHashCode() && c[i].Type == type)
                    return c[i];

            return null;
        }

        private static IEnumerable<IFactoryTypeInfo> GetFactoryTypeInfos(string contractName)
        {
            var c = components;
            for (int i = 0; i < c.Length; i++)
                if (c[i].ContractName.GetHashCode() == contractName.GetHashCode() && c[i].ContractName == contractName)
                    yield return c[i];
        }

        private static object GetInstance(string contractName, object[] parameters)
        {
            var factoryTypeInfos = GetFactoryTypeInfos(contractName);
            var factoryTypeInfosCount = factoryTypeInfos.Count();

            if (factoryTypeInfosCount == 0)
            {
                try
                {
                    // Try to find out the type
                    var realType = Assemblies.GetTypeFromName(contractName);

                    if (realType == null)
                    {
                        if (CanRaiseExceptions)
                            throw new NotImplementedException("The contractName '" + contractName + "' was not found.");
                        else
                            Output.WriteLineError("The contractName '" + contractName + "' was not found.");

                        return null;
                    }

                    return realType.CreateInstance(parameters);
                }
                catch (Exception e)
                {
                    if (CanRaiseExceptions)
                        throw;
                    else
                        Output.WriteLineError(e.Message);

                    return null;
                }
            }

            if (factoryTypeInfosCount > 1)
            {
                var c = factoryExtensions;
                for (int i = 0; i < c.Length; i++)
                {
                    if (c[i].CanHandleAmbiguousMatch)
                    {
                        var selectedType = c[i].SelectAmbiguousMatch(factoryTypeInfos.Select(x => x.Type), contractName);

                        if (selectedType == null)
                            continue;

                        return CreateInstance(selectedType, parameters);
                    }
                }

                throw new AmbiguousMatchException("There is more than one implementation with contractname '" + contractName + "' found.");
            }

            return GetInstance(factoryTypeInfos.First(), parameters);
        }

        private static IEnumerable GetInstances(string contractName, object[] parameters)
        {
            var factoryTypeInfos = components.Where(x => x.ContractName.GetHashCode() == contractName.GetHashCode() && x.ContractName == contractName);

            if (factoryTypeInfos == null)
                throw new KeyNotFoundException("The contractName '" + contractName + "' was not found.");

            var result = new List<object>();

            foreach (var factoryTypeInfo in factoryTypeInfos)
                result.Add(GetInstance(factoryTypeInfo, parameters));

            return result;
        }

        private class FactoryInstancedObject : DisposableBase
        {
            public IFactoryTypeInfo FactoryTypeInfo { get; set; }

            public object Item { get; set; }

            protected override void OnDispose(bool disposeManaged)
            {
                if (disposeManaged)
                    this.Item.TryDispose();
            }
        }
    }

    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class FactoryTypeInfoInternal : IFactoryTypeInfo
    {
        private Func<object[], object> createInstance;

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal FactoryTypeInfoInternal(string contractName, FactoryCreationPolicy creationPolicy, Type type, Func<object[], object> createInstance)
        {
            this.ContractName = contractName;
            this.CreationPolicy = creationPolicy;
            this.Type = type;
            this.createInstance = createInstance;
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ContractName { get; private set; }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public FactoryCreationPolicy CreationPolicy { get; private set; }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Type Type { get; private set; }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object CreateInstance(params object[] arguments) => this.createInstance(arguments);
    }
}