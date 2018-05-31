using Cauldron.Core;
using Cauldron.Core.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Cauldron.Activator
{
    using Cauldron.Core.Diagnostics;

    /// <summary>
    /// Provides methods for creating and destroying object instances
    /// </summary>
    public sealed class Factory
    {
        private static readonly string iFactoryExtensionName = typeof(IFactoryExtension).FullName;
        private static FactoryDictionary<FactoryDictionaryValue> components;
        private static IFactoryTypeInfo[] factoryInfoTypes;

        static Factory()
        {
            if (FactoryCore._components != null)
                factoryInfoTypes = FactoryCore._components;
            else
            {
                var componentList = new List<IFactoryTypeInfo>();
                var getComponents = Assemblies.Known
                        .Select(x => x?
                            .GetType("CauldronInterceptionHelper")?
                            .GetMethod("GetComponents", BindingFlags.Public | BindingFlags.Static))
                            .Where(x => x != null);

                foreach (var item in getComponents)
                    componentList.AddRange(item.Invoke(null, null) as IFactoryTypeInfo[]);

                factoryInfoTypes = componentList.Distinct(new FactoryTypeInfoComparer()).ToArray();
            }

            InitializeFactory(factoryInfoTypes);

            Assemblies.LoadedAssemblyChanged += (s, e) =>
            {
                var factoryComponentsInstances = new List<IFactoryTypeInfo>();
                for (int i = 0; i < e.CauldronGetComponents.Length; i++)
                    factoryComponentsInstances.AddRange(e.CauldronGetComponents[i].Invoke(null, null) as IFactoryTypeInfo[]);

                factoryInfoTypes = factoryInfoTypes.Concat(factoryComponentsInstances).Distinct(new FactoryTypeInfoComparer()).ToArray();
                InitializeFactory(factoryInfoTypes);
            };
        }

        /// <summary>
        /// Occures if an object was created. This will only be invoked if the created object has set its <see cref="ComponentAttribute.InvokeOnObjectCreationEvent"/> to true.
        /// </summary>
        public static event EventHandler<FactoryObjectCreatedEventArgs> ObjectCreated;

        /// <summary>
        /// Occures after the <see cref="Factory"/> was initialized or reloaded.
        /// A Factory reload always occures if <see cref="Assemblies.LoadedAssemblyChanged"/> is invoked.
        /// </summary>
        public static event EventHandler Rebuilt;

        /// <summary>
        /// Gets or sets a value that indicates if the <see cref="Factory"/> is allowed to raise an
        /// exception or not.
        /// </summary>
        public static bool CanRaiseExceptions { get; set; } = true;

        /// <summary>
        /// Gets a collection of <see cref="IFactoryTypeInfo"/> that was found on the entrance assembly and its referencing assemblies.
        /// </summary>
        public static IEnumerable<IFactoryTypeInfo> FactoryTypes => factoryInfoTypes;

        /// <summary>
        /// Gets a collection types that is known to the <see cref="Factory"/>
        /// </summary>
        public static IEnumerable<IFactoryTypeInfo> RegisteredTypes => components.GetValues().SelectMany(x => x.factoryTypeInfos);

        /// <summary>
        /// Gets a collection of resolvers.
        /// The resolvers are only in effect if there are multiple implementations with the same contract name.
        /// </summary>
        public static FactoryResolver Resolvers { get; } = new FactoryResolver();

        /// <summary>
        /// Adds a new <see cref="Type"/> to list of known types.
        /// </summary>
        /// <threadsafety static="false" instance="false"/>
        /// <param name="contractType">The Type that contract name derives from</param>
        /// <param name="creationPolicy">The creation policy of the type as defined by <see cref="FactoryCreationPolicy"/></param>
        /// <param name="type">The type to be added</param>
        /// <param name="createInstance">
        /// An action that is called by the factory to create the object
        /// </param>
        public static IFactoryTypeInfo AddType(Type contractType, FactoryCreationPolicy creationPolicy, Type type, Func<object[], object> createInstance) =>
            AddType(contractType.FullName, creationPolicy, type, createInstance);

        /// <summary>
        /// Adds a new <see cref="Type"/> to list of known types.
        /// </summary>
        /// <threadsafety static="false" instance="false"/>
        /// <param name="contractName">The name that identifies the type</param>
        /// <param name="creationPolicy">The creation policy of the type as defined by <see cref="FactoryCreationPolicy"/></param>
        /// <param name="type">The type to be added</param>
        /// <param name="createInstance">
        /// An action that is called by the factory to create the object
        /// </param>
        public static IFactoryTypeInfo AddType(string contractName, FactoryCreationPolicy creationPolicy, Type type, Func<object[], object> createInstance)
        {
            var factoryTypeInfo = new FactoryTypeInfoInternal(contractName, creationPolicy, type, createInstance);

            if (components.TryGetValue(contractName, out FactoryDictionaryValue content))
            {
                content.factoryTypeInfos = content.factoryTypeInfos.Concat(factoryTypeInfo);
                return factoryTypeInfo;
            }

            components.Add(contractName, new FactoryDictionaryValue
            {
                factoryTypeInfos = new IFactoryTypeInfo[] { factoryTypeInfo }
            });
            return factoryTypeInfo;
        }

        /// <summary>
        /// Creates an instance of the specified type depending on the <see cref="ComponentAttribute"/>
        /// </summary>
        /// <typeparam name="T">The Type that contract name derives from</typeparam>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of the
        /// constructor to invoke. If args is an empty array or null, the constructor that takes no
        /// parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A reference to the newly created object.</returns>
        /// <exception cref="KeyNotFoundException">
        /// The contract described by <typeparamref name="T"/> was not found
        /// </exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="AmbiguousMatchException">
        /// There is more than one implementation with contractname <typeparamref name="T"/> found.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an
        /// implemented <see cref="IDisposable"/> must also implement the <see
        /// cref="IDisposableObject"/> interface.
        /// </exception>
        public static T Create<T>(params object[] parameters) where T : class => GetInstance(typeof(T).FullName, parameters) as T;

        /// <summary>
        /// Creates an instance of the specified type depending on the <see cref="ComponentAttribute"/>
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of the
        /// constructor to invoke. If args is an empty array or null, the constructor that takes no
        /// parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A reference to the newly created object.</returns>
        /// <exception cref="KeyNotFoundException">
        /// The contract described by <paramref name="contractName"/> was not found
        /// </exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="AmbiguousMatchException">
        /// There is more than one implementation with contractname <paramref name="contractName"/> found.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an
        /// implemented <see cref="IDisposable"/> must also implement the <see
        /// cref="IDisposableObject"/> interface.
        /// </exception>
        public static object Create(string contractName, params object[] parameters) => GetInstance(contractName, parameters);

        /// <summary>
        /// Creates an instance of the specified type depending on the <see cref="ComponentAttribute"/>
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from</param>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of the
        /// constructor to invoke. If args is an empty array or null, the constructor that takes no
        /// parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A reference to the newly created object.</returns>
        /// <exception cref="ArgumentNullException">
        /// The parameter <paramref name="contractType"/> is null
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// The contract described by <paramref name="contractType"/> was not found
        /// </exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="AmbiguousMatchException">
        /// There is more than one implementation with contractname <paramref name="contractType"/> found.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an
        /// implemented <see cref="IDisposable"/> must also implement the <see
        /// cref="IDisposableObject"/> interface.
        /// </exception>
        public static object Create(Type contractType, params object[] parameters) => GetInstance(contractType.FullName, parameters);

        /// <summary>
        /// Creates an instance of the specified type depending on the <see cref="ComponentAttribute"/>.
        /// If multiple implementations are available, the <see cref="Factory"/> will prefer the implementation with the highest priority.
        /// </summary>
        /// <typeparam name="T">The Type that contract name derives from</typeparam>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of the
        /// constructor to invoke. If args is an empty array or null, the constructor that takes no
        /// parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A reference to the newly created object.</returns>
        /// <exception cref="KeyNotFoundException">
        /// The contract described by <typeparamref name="T"/> was not found
        /// </exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="NotSupportedException">
        /// An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an
        /// implemented <see cref="IDisposable"/> must also implement the <see cref="IDisposableObject"/> interface.
        /// </exception>
        public static T CreateFirst<T>(params object[] parameters) where T : class => CreateFirst(typeof(T).FullName, parameters) as T;

        /// <summary>
        /// Creates an instance of the specified type depending on the <see cref="ComponentAttribute"/>.
        /// If multiple implementations are available, the <see cref="Factory"/> will prefer the implementation with the highest priority.
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from</param>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of the
        /// constructor to invoke. If args is an empty array or null, the constructor that takes no
        /// parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A reference to the newly created object.</returns>
        /// <exception cref="ArgumentNullException">
        /// The parameter <paramref name="contractType"/> is null
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// The contract described by <paramref name="contractType"/> was not found
        /// </exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="NotSupportedException">
        /// An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an
        /// implemented <see cref="IDisposable"/> must also implement the <see cref="IDisposableObject"/> interface.
        /// </exception>
        public static object CreateFirst(Type contractType, params object[] parameters) => CreateFirst(contractType.FullName, parameters);

        /// <summary>
        /// Creates an instance of the specified type depending on the <see cref="ComponentAttribute"/>.
        /// If multiple implementations are available, the <see cref="Factory"/> will prefer the implementation with the highest priority.
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of the
        /// constructor to invoke. If args is an empty array or null, the constructor that takes no
        /// parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A reference to the newly created object.</returns>
        /// <exception cref="KeyNotFoundException">
        /// The contract described by <paramref name="contractName"/> was not found
        /// </exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="NotSupportedException">
        /// An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an
        /// implemented <see cref="IDisposable"/> must also implement the <see cref="IDisposableObject"/> interface.
        /// </exception>
        public static object CreateFirst(string contractName, params object[] parameters)
        {
            if (components.TryGetValue(contractName, out FactoryDictionaryValue factoryInfos))
            {
                if (factoryInfos.factoryTypeInfos.Length == 1)
                    return factoryInfos.factoryTypeInfos[0].CreateInstance(parameters);

                if (factoryInfos.factoryTypeInfos.Length != 0)
                    return factoryInfos.factoryTypeInfos.MaxBy(x => x.Priority).CreateInstance(parameters);
            }

            if (CanRaiseExceptions)
                throw new KeyNotFoundException("The contractName '" + contractName + "' was not found.");

            Debug.WriteLine($"ERROR: The contractName '" + contractName + "' was not found.");
            return null;
        }

        /// <summary>
        /// Creates an instance of the specified type using the constructor that best matches the
        /// specified parameters. This method is similar to <see
        /// cref="ExtensionsReflection.CreateInstance(Type, object[])"/>, but this takes the types
        /// defined with <see cref="ComponentAttribute"/> into account.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        /// <param name="args">
        /// An array of arguments that match in number, order, and type the parameters of the
        /// constructor to invoke. If args is an empty array or null, the constructor that takes no
        /// parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A reference to the newly created object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null</exception>
        /// <exception cref="NotImplementedException">
        /// Implementation of <paramref name="type"/> not found
        /// </exception>
        public static object CreateInstance(Type type, params object[] args)
        {
            var contractName = type.FullName;
            if (components.TryGetValue(contractName, out FactoryDictionaryValue factoryInfos))
            {
                if (factoryInfos.factoryTypeInfos.Length == 1)
                    return factoryInfos.factoryTypeInfos[0].CreateInstance(args);

                if (factoryInfos.factoryTypeInfos.Length == 0)
                {
                    if (CanRaiseExceptions)
                        throw new NotImplementedException("The contractName '" + contractName + "' was not found.");
                    else
                        Debug.WriteLine($"ERROR: The contractName '{contractName}' was not found.");
                }

                return ResolveAmbiguousMatch(contractName)?.CreateInstance(args);
            }

            return type.CreateInstance(args);
        }

        /// <summary>
        /// Creates instances of the specified type depending on the <see cref="ComponentAttribute"/>
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of the
        /// constructor to invoke. If args is an empty array or null, the constructor that takes no
        /// parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A collection of the newly created objects.</returns>
        /// <exception cref="KeyNotFoundException">
        /// The contract described by <paramref name="contractName"/> was not found
        /// </exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="NotSupportedException">
        /// An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an
        /// implemented <see cref="IDisposable"/> must also implement the <see
        /// cref="IDisposableObject"/> interface.
        /// </exception>
        public static IEnumerable CreateMany(string contractName, params object[] parameters) => GetInstances(contractName, parameters).ToArray();

        /// <summary>
        /// Creates instances of the specified type depending on the <see cref="ComponentAttribute"/>
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from</param>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of the
        /// constructor to invoke. If args is an empty array or null, the constructor that takes no
        /// parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A collection of the newly created objects.</returns>
        /// <exception cref="KeyNotFoundException">
        /// The contract described by <paramref name="contractType"/> was not found
        /// </exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="NotSupportedException">
        /// An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an
        /// implemented <see cref="IDisposable"/> must also implement the <see
        /// cref="IDisposableObject"/> interface.
        /// </exception>
        public static IEnumerable CreateMany(Type contractType, params object[] parameters) => GetInstances(contractType.FullName, parameters).ToArray();

        /// <summary>
        /// Creates instances of the specified type depending on the <see cref="ComponentAttribute"/>
        /// </summary>
        /// <typeparam name="T">The Type that contract name derives from</typeparam>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of the
        /// constructor to invoke. If args is an empty array or null, the constructor that takes no
        /// parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A collection of the newly created objects.</returns>
        /// <exception cref="KeyNotFoundException">
        /// The contract described by <typeparamref name="T"/> was not found
        /// </exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="NotSupportedException">
        /// An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an
        /// implemented <see cref="IDisposable"/> must also implement the <see
        /// cref="IDisposableObject"/> interface.
        /// </exception>
        public static IEnumerable<T> CreateMany<T>(params object[] parameters) => GetInstances(typeof(T).FullName, parameters).Cast<T>();

        /// <summary>
        /// Creates instances of the specified type depending on the <see cref="ComponentAttribute"/>.
        /// The returned values are ordered by the defined priority.
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of the
        /// constructor to invoke. If args is an empty array or null, the constructor that takes no
        /// parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A collection of the newly created objects.</returns>
        /// <exception cref="KeyNotFoundException">
        /// The contract described by <paramref name="contractName"/> was not found
        /// </exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="NotSupportedException">
        /// An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an
        /// implemented <see cref="IDisposable"/> must also implement the <see
        /// cref="IDisposableObject"/> interface.
        /// </exception>
        public static IEnumerable CreateManyOrdered(string contractName, params object[] parameters) => GetInstancesOrdered(contractName, parameters).ToArray();

        /// <summary>
        /// Creates instances of the specified type depending on the <see cref="ComponentAttribute"/>
        /// The returned values are ordered by the defined priority.
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from</param>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of the
        /// constructor to invoke. If args is an empty array or null, the constructor that takes no
        /// parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A collection of the newly created objects.</returns>
        /// <exception cref="KeyNotFoundException">
        /// The contract described by <paramref name="contractType"/> was not found
        /// </exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="NotSupportedException">
        /// An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an
        /// implemented <see cref="IDisposable"/> must also implement the <see
        /// cref="IDisposableObject"/> interface.
        /// </exception>
        public static IEnumerable CreateManyOrdered(Type contractType, params object[] parameters) => GetInstancesOrdered(contractType.FullName, parameters).ToArray();

        /// <summary>
        /// Creates instances of the specified type depending on the <see cref="ComponentAttribute"/>
        /// The returned values are ordered by the defined priority.
        /// </summary>
        /// <typeparam name="T">The Type that contract name derives from</typeparam>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of the
        /// constructor to invoke. If args is an empty array or null, the constructor that takes no
        /// parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A collection of the newly created objects.</returns>
        /// <exception cref="KeyNotFoundException">
        /// The contract described by <typeparamref name="T"/> was not found
        /// </exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="NotSupportedException">
        /// An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an
        /// implemented <see cref="IDisposable"/> must also implement the <see
        /// cref="IDisposableObject"/> interface.
        /// </exception>
        public static IEnumerable<T> CreateManyOrdered<T>(params object[] parameters) => GetInstancesOrdered(typeof(T).FullName, parameters).Cast<T>().ToArray();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// <para/>
        /// Not threadsafe.
        /// </summary>
        /// <typeparam name="T">The Type that the contract name derives from</typeparam>
        public static void Destroy<T>() => Destroy(typeof(T).FullName);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// <para/>
        /// Not threadsafe.
        /// </summary>
        /// <param name="contractType">The Type that the contract name derives from</param>
        public static void Destroy(Type contractType) => Destroy(contractType.FullName);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// <para/>
        /// Not threadsafe.
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        public static void Destroy(string contractName)
        {
            if (components.TryGetValue(contractName, out FactoryDictionaryValue content))
                for (int i = 0; i < content.factoryTypeInfos.Length; i++)
                {
                    (content.factoryTypeInfos[i].Instance as IDisposable)?.Dispose();
                    content.factoryTypeInfos[i].Instance = null;
                }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// <para/>
        /// Not threadsafe.
        /// </summary>
        public static void Destroy()
        {
            foreach (var item in components.GetValues())
                for (int i = 0; i < item.factoryTypeInfos.Length; i++)
                {
                    (item.factoryTypeInfos[i].Instance as IDisposable)?.Dispose();
                    item.factoryTypeInfos[i].Instance = null;
                }
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IFactoryTypeInfo GetFactoryTypeInfo(string contractName)
        {
            if (components.TryGetValue(contractName, out FactoryDictionaryValue factoryInfos))
            {
                if (factoryInfos.factoryTypeInfos.Length == 1)
                    return factoryInfos.factoryTypeInfos[0];

                if (factoryInfos.factoryTypeInfos.Length == 0)
                {
                    if (CanRaiseExceptions)
                        throw new NotImplementedException("The contractName '" + contractName + "' was not found.");
                    else
                        Debug.WriteLine($"ERROR: The contractName '{contractName}' was not found.");
                }

                return ResolveAmbiguousMatch(contractName);
            }

            throw new NotImplementedException("The contractName '" + contractName + "' was not found.");
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IFactoryTypeInfo GetFactoryTypeInfoFirst(string contractName)
        {
            if (components.TryGetValue(contractName, out FactoryDictionaryValue factoryInfos))
            {
                if (factoryInfos.factoryTypeInfos.Length == 1)
                    return factoryInfos.factoryTypeInfos[0];

                if (factoryInfos.factoryTypeInfos.Length == 0)
                {
                    if (CanRaiseExceptions)
                        throw new NotImplementedException("The contractName '" + contractName + "' was not found.");
                    else
                        Debug.WriteLine($"ERROR: The contractName '{contractName}' was not found.");
                }

                return factoryInfos.factoryTypeInfos.MaxBy(x => x.Priority);
            }

            throw new NotImplementedException("The contractName '" + contractName + "' was not found.");
        }

        /// <summary>
        /// Determines whether a contract exist
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        /// <returns>True if the contract exists, otherwise false</returns>
        public static bool HasContract(string contractName) => components.ContainsKey(contractName);

        /// <summary>
        /// Determines whether a contract exist
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from</param>
        /// <returns>True if the contract exists, otherwise false</returns>
        public static bool HasContract(Type contractType) => components.ContainsKey(contractType.FullName);

        /// <summary>
        /// Determines whether a contract exist
        /// </summary>
        /// <typeparam name="T">The Type that contract name derives from</typeparam>
        /// <returns>True if the contract exists, otherwise false</returns>
        public static bool HasContract<T>() => components.ContainsKey(typeof(T).FullName);

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void OnObjectCreation(object @object, IFactoryTypeInfo factoryTypeInfo) => ObjectCreated?.Invoke(null, new FactoryObjectCreatedEventArgs(@object, factoryTypeInfo));

        /// <summary>
        /// Removes a <see cref="Type"/> from the list of known types.
        /// <para/>
        /// Not threadsafe.
        /// </summary>
        /// <param name="contractType">The Type that the contract name derives from</param>
        /// <param name="type">The type to be removed</param>
        /// <threadsafety static="false" instance="false"/>
        public static void RemoveType(Type contractType, Type type) => RemoveType(contractType.FullName, type);

        /// <summary>
        /// Removes a <see cref="Type"/> from the list of known types.
        /// <para/>
        /// Not threadsafe.
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        /// <param name="type">The type to be removed</param>
        /// <threadsafety static="false" instance="false"/>
        public static void RemoveType(string contractName, Type type)
        {
            if (components.TryGetValue(contractName, out FactoryDictionaryValue factoryTypeInfos))
            {
                if (factoryTypeInfos == null)
                    return;

                var tobeRemoved = factoryTypeInfos.factoryTypeInfos.Where(x => x.Type == type).ToArray();
                factoryTypeInfos.factoryTypeInfos = factoryTypeInfos.factoryTypeInfos.Where(x => x.Type != type).ToArray();

                for (int i = 0; i < tobeRemoved.Length; i++)
                {
                    (tobeRemoved[i].Instance as IDisposable)?.Dispose();
                    tobeRemoved[i].Instance = null;
                }
            }
        }

        private static void GetAndInitializeAllExtensions(IEnumerable<IFactoryTypeInfo> factoryTypeInfos)
        {
            foreach (var item in factoryInfoTypes
                  .Where(x => x.ContractName.GetHashCode() == iFactoryExtensionName.GetHashCode() && x.ContractName == iFactoryExtensionName).Select(x => x.CreateInstance() as IFactoryExtension))
                item.Initialize(factoryTypeInfos);
        }

        private static object GetInstance(string contractName, object[] parameters)
        {
            if (components.TryGetValue(contractName, out FactoryDictionaryValue factoryInfos))
            {
                if (factoryInfos.factoryTypeInfos.Length == 1)
                    return factoryInfos.factoryTypeInfos[0].CreateInstance(parameters);

                if (factoryInfos.factoryTypeInfos.Length == 0)
                {
                    if (CanRaiseExceptions)
                        throw new NotImplementedException("The contractName '" + contractName + "' was not found.");
                    else
                        Debug.WriteLine($"ERROR: The contractName '{contractName}' was not found.");
                }

                return ResolveAmbiguousMatch(contractName)?.CreateInstance(parameters);
            }

            try
            {
                // Try to find out the type
                var realType = Assemblies.GetTypeFromName(contractName);

                if (realType == null)
                {
                    if (CanRaiseExceptions)
                        throw new NotImplementedException("The contractName '" + contractName + "' was not found.");
                    else
                        Debug.WriteLine($"ERROR: The contractName '{contractName}' was not found.");

                    return null;
                }

                return realType.CreateInstance(parameters);
            }
            catch (Exception e)
            {
                if (CanRaiseExceptions)
                    throw;
                else
                    Debug.WriteLine(e.Message);

                return null;
            }
        }

        private static object[] GetInstances(string contractName, object[] parameters)
        {
            if (components.TryGetValue(contractName, out FactoryDictionaryValue factoryInfos))
            {
                var result = new object[factoryInfos.factoryTypeInfos.Length];
                for (int i = 0; i < factoryInfos.factoryTypeInfos.Length; i++)
                    result[i] = factoryInfos.factoryTypeInfos[i].CreateInstance(parameters);

                return result;
            }

            if (CanRaiseExceptions)
                throw new NotImplementedException("The contractName '" + contractName + "' was not found.");
            else
                Debug.WriteLine($"ERROR: The contractName '" + contractName + "' was not found.");

            return null;
        }

        private static object[] GetInstancesOrdered(string contractName, object[] parameters)
        {
            if (components.TryGetValue(contractName, out FactoryDictionaryValue factoryInfos))
            {
                var result = new object[factoryInfos.factoryTypeInfos.Length];
                int i = 0;
                foreach (var item in factoryInfos.factoryTypeInfos.OrderBy(x => x.Priority))
                    result[i++] = factoryInfos.factoryTypeInfos[i].CreateInstance(parameters);

                return result;
            }

            if (CanRaiseExceptions)
                throw new NotImplementedException("The contractName '" + contractName + "' was not found.");

            Debug.WriteLine($"ERROR: The contractName '" + contractName + "' was not found.");

            return null;
        }

        private static void InitializeFactory(IEnumerable<IFactoryTypeInfo> factoryInfoTypes)
        {
            // Get all known components
            components = new FactoryDictionary<FactoryDictionaryValue>();
            foreach (var item in factoryInfoTypes.GroupBy(x => x.ContractName).Select(x => new { x.Key, Items = x.ToArray() }))
                components.Add(item.Key, new FactoryDictionaryValue { factoryTypeInfos = item.Items });
            // Get all factory extensions
            GetAndInitializeAllExtensions(factoryInfoTypes);

            if (components.Count == 0)
                Debug.WriteLine($"ERROR: Unable to find any components. Please check if FodyWeavers.xml has an entry for Cauldron.Interception");

            Rebuilt?.Invoke(null, EventArgs.Empty);
        }

        private static IFactoryTypeInfo ResolveAmbiguousMatch(string contractName) => Resolvers.SelectAmbiguousMatch(contractName);
    }
}