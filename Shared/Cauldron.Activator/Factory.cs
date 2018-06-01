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
    public static class Factory
    {
        private static readonly string iFactoryExtensionName = typeof(IFactoryExtension).FullName;
        private static FactoryStringDictionary<FactoryDictionaryValue> componentsNamed;
        private static FactoryDictionary<Type, FactoryDictionaryValue> componentsTyped;
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
        public static IEnumerable<IFactoryTypeInfo> RegisteredTypes => componentsNamed.GetValues().SelectMany(x => x.factoryTypeInfos);

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
            var result = componentsNamed[contractName];

            if (result == null)
                componentsNamed.Add(contractName, new FactoryDictionaryValue().SetValues(new IFactoryTypeInfo[] { factoryTypeInfo }));
            else
                result.SetValues(result.factoryTypeInfos.Concat(factoryTypeInfo).ToArray());

            return factoryTypeInfo;
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
            var factoryInfos = componentsNamed[type.FullName];
            if (factoryInfos == null)
            {
#if NETFX_CORE
                if(type.GetTypeInfo().IsInterface)
#else
                if (type.IsInterface)
#endif
                {
                    if (CanRaiseExceptions)
                        throw new NotImplementedException($"The contractName '{type}' was not found.");

                    Debug.WriteLine($"ERROR: The contractName '{type}' was not found.");
                    return null;
                }

                return type.CreateInstance(args);
            }

            if (factoryInfos.isSingle)
                return factoryInfos.ambigious.CreateInstance(args);

            var result = ResolveAmbiguousMatch(type.FullName);
            factoryInfos.ambigious = result;
            factoryInfos.isSingle = true;

            return result.CreateInstance(args);
        }

        #region Create

        /// <summary>
        /// Creates an instance of the specified type depending on the <see cref="ComponentAttribute"/>
        /// </summary>
        /// <typeparam name="T">The Type that contract name derives from</typeparam>
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
        public static T Create<T>() where T : class => Create(typeof(T)) as T;

        /// <summary>
        /// Creates an instance of the specified type depending on the <see cref="ComponentAttribute"/>
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
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
        public static object Create(string contractName)
        {
            var factoryInfos = componentsNamed[contractName];
            if (factoryInfos == null)
            {
                if (CanRaiseExceptions)
                    throw new NotImplementedException($"The contractName '{contractName}' was not found.");

                Debug.WriteLine($"ERROR: The contractName '{contractName}' was not found.");
                return null;
            }

            if (factoryInfos.isSingle)
                return factoryInfos.ambigious.CreateInstance();

            var result = ResolveAmbiguousMatch(contractName);
            factoryInfos.ambigious = result;
            factoryInfos.isSingle = true;

            return result.CreateInstance();
        }

        /// <summary>
        /// Creates an instance of the specified type depending on the <see cref="ComponentAttribute"/>
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from</param>
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
        public static object Create(Type contractType)
        {
            var factoryInfos = componentsTyped[contractType];
            if (factoryInfos == null)
            {
                if (CanRaiseExceptions)
                    throw new NotImplementedException($"The contractName '{contractType.FullName}' was not found.");

                Debug.WriteLine($"ERROR: The contractName '{contractType.FullName}' was not found.");
                return null;
            }

            if (factoryInfos.isSingle)
                return factoryInfos.ambigious.CreateInstance();

            var result = ResolveAmbiguousMatch(contractType.FullName);
            factoryInfos.ambigious = result;
            factoryInfos.isSingle = true;

            return result.CreateInstance();
        }

        #endregion Create

        #region CreateFirst

        /// <summary>
        /// Creates an instance of the specified type depending on the <see cref="ComponentAttribute"/>.
        /// If multiple implementations are available, the <see cref="Factory"/> will prefer the implementation with the highest priority.
        /// </summary>
        /// <typeparam name="T">The Type that contract name derives from</typeparam>
        /// <returns>A reference to the newly created object.</returns>
        /// <exception cref="KeyNotFoundException">
        /// The contract described by <typeparamref name="T"/> was not found
        /// </exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="NotSupportedException">
        /// An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an
        /// implemented <see cref="IDisposable"/> must also implement the <see cref="IDisposableObject"/> interface.
        /// </exception>
        public static T CreateFirst<T>() where T : class => CreateFirst(typeof(T)) as T;

        /// <summary>
        /// Creates an instance of the specified type depending on the <see cref="ComponentAttribute"/>.
        /// If multiple implementations are available, the <see cref="Factory"/> will prefer the implementation with the highest priority.
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from</param>
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
        public static object CreateFirst(Type contractType)
        {
            var factoryInfos = componentsTyped[contractType];
            if (factoryInfos == null)
            {
                if (CanRaiseExceptions)
                    throw new NotImplementedException($"The contractName '{contractType}' was not found.");

                Debug.WriteLine($"ERROR: The contractName '{contractType}' was not found.");
                return null;
            }

            return factoryInfos.createFirst.CreateInstance();
        }

        /// <summary>
        /// Creates an instance of the specified type depending on the <see cref="ComponentAttribute"/>.
        /// If multiple implementations are available, the <see cref="Factory"/> will prefer the implementation with the highest priority.
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        /// <returns>A reference to the newly created object.</returns>
        /// <exception cref="KeyNotFoundException">
        /// The contract described by <paramref name="contractName"/> was not found
        /// </exception>
        /// <exception cref="Exception">Unknown <see cref="FactoryCreationPolicy"/></exception>
        /// <exception cref="NotSupportedException">
        /// An <see cref="object"/> with <see cref="FactoryCreationPolicy.Singleton"/> with an
        /// implemented <see cref="IDisposable"/> must also implement the <see cref="IDisposableObject"/> interface.
        /// </exception>
        public static object CreateFirst(string contractName)
        {
            var factoryInfos = componentsNamed[contractName];
            if (factoryInfos == null)
            {
                if (CanRaiseExceptions)
                    throw new NotImplementedException($"The contractName '{contractName}' was not found.");

                Debug.WriteLine($"ERROR: The contractName '{contractName}' was not found.");
                return null;
            }

            return factoryInfos.createFirst.CreateInstance();
        }

        #endregion CreateFirst

        #region CreateMany

        /// <summary>
        /// Creates instances of the specified type depending on the <see cref="ComponentAttribute"/>
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
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
        public static IEnumerable CreateMany(string contractName)
        {
            var factoryInfos = componentsNamed[contractName];
            if (factoryInfos == null)
            {
                if (CanRaiseExceptions)
                    throw new NotImplementedException($"The contractName '{contractName}' was not found.");

                Debug.WriteLine($"ERROR: The contractName '{contractName}' was not found.");
                return null;
            }

            var result = new object[factoryInfos.factoryTypeInfos.Length];
            for (int i = 0; i < factoryInfos.factoryTypeInfos.Length; i++)
                result[i] = factoryInfos.factoryTypeInfos[i].CreateInstance();

            return result;
        }

        /// <summary>
        /// Creates instances of the specified type depending on the <see cref="ComponentAttribute"/>
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from</param>
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
        public static IEnumerable CreateMany(Type contractType)
        {
            var factoryInfos = componentsTyped[contractType];
            if (factoryInfos == null)
            {
                if (CanRaiseExceptions)
                    throw new NotImplementedException($"The contractName '{contractType}' was not found.");

                Debug.WriteLine($"ERROR: The contractName '{contractType}' was not found.");
                return null;
            }

            var result = new object[factoryInfos.factoryTypeInfos.Length];
            for (int i = 0; i < factoryInfos.factoryTypeInfos.Length; i++)
                result[i] = factoryInfos.factoryTypeInfos[i].CreateInstance();

            return result;
        }

        /// <summary>
        /// Creates instances of the specified type depending on the <see cref="ComponentAttribute"/>
        /// </summary>
        /// <typeparam name="T">The Type that contract name derives from</typeparam>
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
        public static IEnumerable<T> CreateMany<T>() => CreateMany(typeof(T)).Cast<T>();

        #endregion CreateMany

        #region Create Many Ordered

        /// <summary>
        /// Creates instances of the specified type depending on the <see cref="ComponentAttribute"/>.
        /// The returned values are ordered by the defined priority.
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
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
        public static IEnumerable CreateManyOrdered(string contractName)
        {
            var factoryInfos = componentsNamed[contractName];
            if (factoryInfos == null)
            {
                if (CanRaiseExceptions)
                    throw new NotImplementedException($"The contractName '{contractName}' was not found.");

                Debug.WriteLine($"ERROR: The contractName '{contractName}' was not found.");
                return null;
            }

            var result = new object[factoryInfos.factoryTypeInfos.Length];
            for (int i = 0; i < factoryInfos.factoryTypeInfos.Length; i++)
                result[i] = factoryInfos.factoryTypeInfosOrdered[i].CreateInstance();

            return result;
        }

        /// <summary>
        /// Creates instances of the specified type depending on the <see cref="ComponentAttribute"/>
        /// The returned values are ordered by the defined priority.
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from</param>
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
        public static IEnumerable CreateManyOrdered(Type contractType)
        {
            var factoryInfos = componentsTyped[contractType];
            if (factoryInfos == null)
            {
                if (CanRaiseExceptions)
                    throw new NotImplementedException($"The contractName '{contractType}' was not found.");

                Debug.WriteLine($"ERROR: The contractName '{contractType}' was not found.");
                return null;
            }

            var result = new object[factoryInfos.factoryTypeInfos.Length];
            for (int i = 0; i < factoryInfos.factoryTypeInfos.Length; i++)
                result[i] = factoryInfos.factoryTypeInfosOrdered[i].CreateInstance();

            return result;
        }

        /// <summary>
        /// Creates instances of the specified type depending on the <see cref="ComponentAttribute"/>
        /// The returned values are ordered by the defined priority.
        /// </summary>
        /// <typeparam name="T">The Type that contract name derives from</typeparam>
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
        public static IEnumerable<T> CreateManyOrdered<T>() => CreateManyOrdered(typeof(T)).Cast<T>();

        #endregion Create Many Ordered

        #region Create with parameters

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
        public static T Create<T>(params object[] parameters) where T : class => Create(typeof(T), parameters) as T;

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
        public static object Create(string contractName, params object[] parameters)
        {
            var factoryInfos = componentsNamed[contractName];
            if (factoryInfos == null)
            {
                if (CanRaiseExceptions)
                    throw new NotImplementedException($"The contractName '{contractName}' was not found.");

                Debug.WriteLine($"ERROR: The contractName '{contractName}' was not found.");
                return null;
            }

            if (factoryInfos.isSingle)
                return factoryInfos.ambigious.CreateInstance(parameters);

            var result = ResolveAmbiguousMatch(contractName);
            factoryInfos.ambigious = result;
            factoryInfos.isSingle = true;

            return result.CreateInstance(parameters);
        }

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
        public static object Create(Type contractType, params object[] parameters)
        {
            var factoryInfos = componentsTyped[contractType];
            if (factoryInfos == null)
            {
                if (CanRaiseExceptions)
                    throw new NotImplementedException($"The contractName '{contractType.FullName}' was not found.");

                Debug.WriteLine($"ERROR: The contractName '{contractType.FullName}' was not found.");
                return null;
            }

            if (factoryInfos.isSingle)
                return factoryInfos.ambigious.CreateInstance(parameters);

            var result = ResolveAmbiguousMatch(contractType.FullName);
            factoryInfos.ambigious = result;
            factoryInfos.isSingle = true;

            return result.CreateInstance(parameters);
        }

        #endregion Create with parameters

        #region CreateFirst with parameters

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
        public static T CreateFirst<T>(params object[] parameters) where T : class => CreateFirst(typeof(T), parameters) as T;

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
        public static object CreateFirst(Type contractType, params object[] parameters)
        {
            var factoryInfos = componentsTyped[contractType];
            if (factoryInfos == null)
            {
                if (CanRaiseExceptions)
                    throw new NotImplementedException($"The contractName '{contractType}' was not found.");

                Debug.WriteLine($"ERROR: The contractName '{contractType}' was not found.");
                return null;
            }

            return factoryInfos.createFirst.CreateInstance(parameters);
        }

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
            var factoryInfos = componentsNamed[contractName];
            if (factoryInfos == null)
            {
                if (CanRaiseExceptions)
                    throw new NotImplementedException($"The contractName '{contractName}' was not found.");

                Debug.WriteLine($"ERROR: The contractName '{contractName}' was not found.");
                return null;
            }

            return factoryInfos.createFirst.CreateInstance(parameters);
        }

        #endregion CreateFirst with parameters

        #region CreateMany with parameters

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
        public static IEnumerable CreateMany(string contractName, params object[] parameters)
        {
            var factoryInfos = componentsNamed[contractName];
            if (factoryInfos == null)
            {
                if (CanRaiseExceptions)
                    throw new NotImplementedException($"The contractName '{contractName}' was not found.");

                Debug.WriteLine($"ERROR: The contractName '{contractName}' was not found.");
                return null;
            }

            var result = new object[factoryInfos.factoryTypeInfos.Length];
            for (int i = 0; i < factoryInfos.factoryTypeInfos.Length; i++)
                result[i] = factoryInfos.factoryTypeInfos[i].CreateInstance(parameters);

            return result;
        }

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
        public static IEnumerable CreateMany(Type contractType, params object[] parameters)
        {
            var factoryInfos = componentsTyped[contractType];
            if (factoryInfos == null)
            {
                if (CanRaiseExceptions)
                    throw new NotImplementedException($"The contractName '{contractType}' was not found.");

                Debug.WriteLine($"ERROR: The contractName '{contractType}' was not found.");
                return null;
            }

            var result = new object[factoryInfos.factoryTypeInfos.Length];
            for (int i = 0; i < factoryInfos.factoryTypeInfos.Length; i++)
                result[i] = factoryInfos.factoryTypeInfos[i].CreateInstance(parameters);

            return result;
        }

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
        public static IEnumerable<T> CreateMany<T>(params object[] parameters) => CreateMany(typeof(T), parameters).Cast<T>();

        #endregion CreateMany with parameters

        #region Create Many Ordered with parameters

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
        public static IEnumerable CreateManyOrdered(string contractName, params object[] parameters)
        {
            var factoryInfos = componentsNamed[contractName];
            if (factoryInfos == null)
            {
                if (CanRaiseExceptions)
                    throw new NotImplementedException($"The contractName '{contractName}' was not found.");

                Debug.WriteLine($"ERROR: The contractName '{contractName}' was not found.");
                return null;
            }

            var result = new object[factoryInfos.factoryTypeInfos.Length];
            for (int i = 0; i < factoryInfos.factoryTypeInfos.Length; i++)
                result[i] = factoryInfos.factoryTypeInfosOrdered[i].CreateInstance(parameters);

            return result;
        }

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
        public static IEnumerable CreateManyOrdered(Type contractType, params object[] parameters)
        {
            var factoryInfos = componentsTyped[contractType];
            if (factoryInfos == null)
            {
                if (CanRaiseExceptions)
                    throw new NotImplementedException($"The contractName '{contractType}' was not found.");

                Debug.WriteLine($"ERROR: The contractName '{contractType}' was not found.");
                return null;
            }

            var result = new object[factoryInfos.factoryTypeInfos.Length];
            for (int i = 0; i < factoryInfos.factoryTypeInfos.Length; i++)
                result[i] = factoryInfos.factoryTypeInfosOrdered[i].CreateInstance(parameters);

            return result;
        }

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
        public static IEnumerable<T> CreateManyOrdered<T>(params object[] parameters) => CreateManyOrdered(typeof(T), parameters).Cast<T>();

        #endregion Create Many Ordered with parameters

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
            var value = componentsNamed[contractName];

            if (value != null)
                for (int i = 0; i < value.factoryTypeInfos.Length; i++)
                {
                    (value.factoryTypeInfos[i].Instance as IDisposable)?.Dispose();
                    value.factoryTypeInfos[i].Instance = null;
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
            foreach (var item in componentsNamed.GetValues())
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
            var factoryInfos = componentsNamed[contractName];
            if (factoryInfos == null)
            {
                if (CanRaiseExceptions)
                    throw new NotImplementedException($"The contractName '{contractName}' was not found.");

                Debug.WriteLine($"ERROR: The contractName '{contractName}' was not found.");
                return null;
            }

            if (factoryInfos.isSingle)
                return factoryInfos.ambigious;

            var result = ResolveAmbiguousMatch(contractName);
            factoryInfos.ambigious = result;
            factoryInfos.isSingle = true;

            return result;
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IFactoryTypeInfo GetFactoryTypeInfoFirst(string contractName)
        {
            var factoryInfos = componentsNamed[contractName];
            if (factoryInfos == null)
            {
                if (CanRaiseExceptions)
                    throw new NotImplementedException($"The contractName '{contractName}' was not found.");

                Debug.WriteLine($"ERROR: The contractName '{contractName}' was not found.");
                return null;
            }

            return factoryInfos.createFirst;
        }

        /// <summary>
        /// Determines whether a contract exist
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        /// <returns>True if the contract exists, otherwise false</returns>
        public static bool HasContract(string contractName) => componentsNamed.ContainsKey(contractName);

        /// <summary>
        /// Determines whether a contract exist
        /// </summary>
        /// <param name="contractType">The Type that contract name derives from</param>
        /// <returns>True if the contract exists, otherwise false</returns>
        public static bool HasContract(Type contractType) => componentsTyped.ContainsKey(contractType);

        /// <summary>
        /// Determines whether a contract exist
        /// </summary>
        /// <typeparam name="T">The Type that contract name derives from</typeparam>
        /// <returns>True if the contract exists, otherwise false</returns>
        public static bool HasContract<T>() => componentsTyped.ContainsKey(typeof(T));

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
            var factoryTypeInfo = componentsNamed[contractName];
            if (factoryTypeInfo == null)
                return;

            var tobeRemoved = factoryTypeInfo.factoryTypeInfos.Where(x => x.Type == type).ToArray();
            var newList = factoryTypeInfo.factoryTypeInfos.Where(x => x.Type != type).ToArray();

            if (newList.Length == 0)
                componentsNamed.Remove(contractName);
            else
                factoryTypeInfo.SetValues(newList);

            for (int i = 0; i < tobeRemoved.Length; i++)
            {
                (tobeRemoved[i].Instance as IDisposable)?.Dispose();
                tobeRemoved[i].Instance = null;
            }
        }

        private static void GetAndInitializeAllExtensions(IEnumerable<IFactoryTypeInfo> factoryTypeInfos)
        {
            foreach (var item in factoryInfoTypes.Where(x => x.ContractName.GetHashCode() == iFactoryExtensionName.GetHashCode() && x.ContractName == iFactoryExtensionName).Select(x => x.CreateInstance() as IFactoryExtension))
                item.Initialize(factoryTypeInfos);
        }

        private static void InitializeFactory(IEnumerable<IFactoryTypeInfo> factoryInfoTypes)
        {
            // Get all known components
            componentsNamed = new FactoryStringDictionary<FactoryDictionaryValue>();
            foreach (var item in factoryInfoTypes.GroupBy(x => x.ContractName).Select(x => new { x.Key, Items = x.ToArray() }))
                componentsNamed.Add(item.Key, new FactoryDictionaryValue().SetValues(item.Items));
            // Get all factory extensions
            GetAndInitializeAllExtensions(factoryInfoTypes);

            if (componentsNamed.Count == 0)
                Debug.WriteLine($"ERROR: Unable to find any components. Please check if FodyWeavers.xml has an entry for Cauldron.Interception");

            // Get all known components
            componentsTyped = new FactoryDictionary<Type, FactoryDictionaryValue>();
            foreach (var item in factoryInfoTypes.Where(x => x.ContractType != null).GroupBy(x => x.ContractType).Select(x => new { x.Key, Items = x.ToArray() }).Where(x => x.Items.Length > 0))
                componentsTyped.Add(item.Key, new FactoryDictionaryValue().SetValues(item.Items));
            // Get all factory extensions
            GetAndInitializeAllExtensions(factoryInfoTypes);

            Rebuilt?.Invoke(null, EventArgs.Empty);
        }

        private static IFactoryTypeInfo ResolveAmbiguousMatch(string contractName) => Resolvers.SelectAmbiguousMatch(contractName);

        private static FactoryDictionaryValue SetValues(this FactoryDictionaryValue factoryDictionaryValue, IFactoryTypeInfo[] items)
        {
            factoryDictionaryValue.factoryTypeInfos = items;
            factoryDictionaryValue.factoryTypeInfosOrdered = items.OrderBy(x => x.Priority).ToArray();
            factoryDictionaryValue.createFirst = items.MaxBy(x => x.Priority);
            factoryDictionaryValue.ambigious = items.Length == 1 ? items[0] : null;
            factoryDictionaryValue.isSingle = items.Length == 1;

            return factoryDictionaryValue;
        }
    }

    internal sealed class FactoryDictionaryValue
    {
        public IFactoryTypeInfo ambigious;
        public IFactoryTypeInfo createFirst;
        public IFactoryTypeInfo[] factoryTypeInfos;
        public IFactoryTypeInfo[] factoryTypeInfosOrdered;
        public bool isSingle;
    }
}