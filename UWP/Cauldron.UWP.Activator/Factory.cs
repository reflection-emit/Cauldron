using Cauldron.Core;
using Cauldron.Core.Collections;
using Cauldron.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.UI.Core;

namespace Cauldron.Activator
{
    /// <summary>
    /// Provides methods for creating and destroying object instances
    /// </summary>
    public sealed class Factory
    {
        private static List<IFactoryExtension> factoryExtensions = new List<IFactoryExtension>();
        private static ConcurrentList<ObjectKey> instances = new ConcurrentList<ObjectKey>();
        private static List<FactoryTypeInfo> types = new List<FactoryTypeInfo>();

        static Factory()
        {
            // Get all factory extensions
            factoryExtensions.AddRange(Assemblies.GetTypesImplementsInterface<IFactoryExtension>()
                .Select(x => System.Activator.CreateInstance(x.AsType()) as IFactoryExtension));

            var attributeType = typeof(ComponentAttribute);
            var definedTypes = Assemblies.ExportedTypes
                .Where(x => !x.IsInterface)
                .Select(x => new Component
                {
                    Attrib = x.GetCustomAttribute(attributeType, false) as ComponentAttribute,
                    TypeInfo = x
                });

            AddTypes(definedTypes);

            Assemblies.LoadedAssemblyChanged += (s, e) =>
            {
                // Get all factory extensions
                var factoryExtensionTypes = Assemblies.GetTypesImplementsInterface<IFactoryExtension>();

                foreach (var item in factoryExtensionTypes)
                    if (!factoryExtensions.Any(x => x.GetType().FullName == item.FullName))
                    {
                        factoryExtensions.Add(System.Activator.CreateInstance(item.AsType()) as IFactoryExtension);

                        AddTypes(Assemblies.ExportedTypes
                            .Where(x => !x.IsInterface && !Factory.types.Any(y => y.typeInfo == x))
                            .Select(x => new Component
                            {
                                Attrib = x.GetCustomAttribute<ComponentAttribute>(false),
                                TypeInfo = x
                            }));
                    }
            };
        }

        /// <summary>
        /// Adds a new <see cref="Type"/> to list of known types.
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        /// <param name="creationPolicy">The creation policy of the type as defined by <see cref="FactoryCreationPolicy"/></param>
        /// <param name="type">The type to be added</param>
        public static FactoryTypeInfo AddType(string contractName, FactoryCreationPolicy creationPolicy, Type type)
        {
            // Check if the type has a predefined object constructor
            var constructor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(x => x.GetCustomAttribute<ComponentConstructorAttribute>() != null);

            MethodInfo methodInfo = null;

            if (constructor == null) // or a method acting as constructor
                methodInfo = type.GetMethodsEx(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(x => x.GetCustomAttribute<ComponentConstructorAttribute>() != null);
            var info = new FactoryTypeInfo(
                 contractName,
                 creationPolicy,
                 constructor,
                 methodInfo,
                 type,
                 type.GetTypeInfo());
            Factory.types.Add(info);

            return info;
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
        public static T Create<T>(params object[] parameters) => (T)GetInstance(typeof(T).FullName, parameters);

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

            // First check if we can use predefined constructors
            var factoryType = Factory.types.FirstOrDefault(x => x.type == type && (x.objectConstructorMethodInfo != null || x.objectConstructorInfo != null));

            try
            {
                if (factoryType != null)
                {
                    if (factoryType.objectConstructorInfo != null)
                    {
                        var ctor = factoryType.objectConstructorInfo;
                        var creatorExtension = factoryExtensions.FirstOrDefault(x => x.CanModifyArguments(ctor, type));

                        return ctor.CreateInstance(
                            creatorExtension == null ?
                            args :
                            creatorExtension.ModifyArgument(
                                factoryType.objectConstructorInfo.GetParameters(),
                                args));
                    }
                    else
                    {
                        var creatorExtension = factoryExtensions.FirstOrDefault(x => x.CanModifyArguments(factoryType.objectConstructorMethodInfo, type));
                        return factoryType.objectConstructorMethodInfo.Invoke(null,
                                creatorExtension == null ?
                                args :
                                creatorExtension.ModifyArgument(
                                    factoryType.objectConstructorMethodInfo.GetParameters(),
                                    args));
                    }
                }
                else
                {
                    // Check if the type has a predefined object constructor
                    var ctor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                        .FirstOrDefault(x => x.GetCustomAttribute<ComponentConstructorAttribute>() != null);

                    if (ctor != null)
                    {
                        var creatorExtension = factoryExtensions.FirstOrDefault(x => x.CanModifyArguments(ctor, type));
                        return ctor.CreateInstance(creatorExtension == null ?
                            args :
                            creatorExtension.ModifyArgument(ctor.GetParameters(), args));
                    }
                    else if (ctor == null) // or a method acting as constructor
                    {
                        var methodInfo = type.GetMethodsEx(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                            .FirstOrDefault(x => x.GetCustomAttribute<ComponentConstructorAttribute>() != null);

                        if (methodInfo != null)
                        {
                            var creatorExtension = factoryExtensions.FirstOrDefault(x => x.CanModifyArguments(methodInfo, type));
                            return methodInfo.Invoke(null, creatorExtension == null ?
                                args :
                                creatorExtension.ModifyArgument(ctor.GetParameters(), args));
                        }
                    }

                    return type.CreateInstance(args);
                }
            }
            catch (CreateInstanceIsAnInterfaceException e)
            {
                Console.WriteLine($"Implementation of '{type.FullName}' not found.");
                throw new NotImplementedException($"Unable to find the implementation of '{type.FullName}'. Make sure that the Assembly with implementation was loaded in the AppDomain.", e);
            }
            catch
            {
                throw;
            }
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
            if (instances.Contains(x => x.FactoryTypeInfo.contractName == contractName))
                instances.Remove(x => x.FactoryTypeInfo.contractName == contractName).TryDispose();
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
        public static bool HasContract(string contractName) => types.Any(x => x.contractName == contractName);

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

            if (types.Any(x => x.contractName == contractName))
                return types.First(x => x.contractName == contractName).creationPolicy == FactoryCreationPolicy.Singleton;

            return null;
        }

        /// <summary>
        /// Removes a <see cref="Type"/> from the list of known types
        /// </summary>
        /// <param name="contractName">The name that identifies the type</param>
        /// <param name="type">The type to be removed</param>
        public static void RemoveType(string contractName, Type type)
        {
            var t = Factory.types.FirstOrDefault(x => x.contractName == contractName && x.type.FullName == type.FullName);

            if (t.type != null)
                Factory.types.Remove(t);
        }

        private static void AddTypes(IEnumerable<Component> definedTypes)
        {
            foreach (var d in definedTypes)
            {
                var type = d.TypeInfo.AsType();

                for (int i = 0; i < factoryExtensions.Count; i++)
                    factoryExtensions[i].OnInitialize(type);

                var contractName = d.Attrib == null ? type.FullName : d.Attrib.ContractName;
                var policy = d.Attrib == null ? FactoryCreationPolicy.Instanced : d.Attrib.Policy;

                AddType(contractName, policy, type);
            }
        }

        private static object CreateObject(Type type, TypeInfo typeInfo, object[] parameters)
        {
            var result = CreateInstance(type, parameters);

            // Activate all Extensions
            for (int i = 0; i < factoryExtensions.Count; i++)
                factoryExtensions[i].OnCreateObject(result, type);

            // Invoke the IFactoryInitializeComponent.OnInitializeComponent method if implemented
            if (result is IFactoryInitializeComponent)
            {
                var dispatcher = DispatcherEx.Current;

#pragma warning disable 4014

                dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                    (result as IFactoryInitializeComponent)?.OnInitializeComponentAsync());
#pragma warning restore 4014
            }

            return result;
        }

        private static object GetInstance(FactoryTypeInfo factoryTypeInfo, object[] parameters)
        {
            if (factoryTypeInfo.creationPolicy == FactoryCreationPolicy.Instanced)
                return CreateObject(factoryTypeInfo.type, factoryTypeInfo.typeInfo, parameters);
            else if (factoryTypeInfo.creationPolicy == FactoryCreationPolicy.Singleton)
            {
                if (instances.Contains(x => x.FactoryTypeInfo.contractName == factoryTypeInfo.contractName && x.FactoryTypeInfo.type.FullName == factoryTypeInfo.type.FullName))
                    return instances.First(x => x.FactoryTypeInfo.contractName == factoryTypeInfo.contractName && x.FactoryTypeInfo.type.FullName == factoryTypeInfo.type.FullName).Item;
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
            if (!types.Any(x => x.contractName == contractName) && !instances.Contains(x => x.FactoryTypeInfo.contractName == contractName))
            {
                try
                {
                    // Try to find out the type
                    var realType = Assemblies.GetTypeFromName(contractName);

                    if (realType == null)
                    {
                        Output.WriteLineError("The contractName '" + contractName + "' was not found.");
                        return null;
                    }

                    var attr = realType.GetTypeInfo().GetCustomAttribute(typeof(ComponentAttribute)) as ComponentAttribute;

                    var info = AddType(
                        contractName,
                        attr == null ? FactoryCreationPolicy.Instanced : attr.Policy,
                        realType);

                    return GetInstance(info, parameters);
                }
                catch (Exception e)
                {
                    Output.WriteLineError(e.Message);
                    return null;
                }
            }

            var factoryTypeInfos = types.Where(x => x.contractName == contractName);

            if (factoryTypeInfos.Count() > 1)
            {
                for (int i = 0; i < factoryExtensions.Count; i++)
                {
                    if (factoryExtensions[i].CanHandleAmbiguousMatch)
                    {
                        var selectedType = factoryExtensions[i].SelectAmbiguousMatch(factoryTypeInfos.Select(x => x.type), contractName);

                        if (selectedType == null)
                            continue;
#if WINDOWS_UWP
                        var selectedFactoryInfo = factoryTypeInfos.FirstOrDefault(x => x.type.FullName == selectedType.FullName && x.type.GetTypeInfo().Assembly.FullName == selectedType.GetTypeInfo().Assembly.FullName);
#else
                        var selectedFactoryInfo = factoryTypeInfos.FirstOrDefault(x => x.type.FullName == selectedType.FullName && x.type.Assembly.FullName == selectedType.Assembly.FullName);
#endif

                        if (selectedFactoryInfo.type != null)
                            return GetInstance(selectedFactoryInfo, parameters);
                    }
                }

                throw new AmbiguousMatchException("There is more than one implementation with contractname '" + contractName + "' found.");
            }

            return GetInstance(factoryTypeInfos.First(), parameters);
        }

        private static IEnumerable GetInstances(string contractName, object[] parameters)
        {
            if (!types.Any(x => x.contractName == contractName) && !instances.Contains(x => x.FactoryTypeInfo.contractName == contractName))
                throw new KeyNotFoundException("The contractName '" + contractName + "' was not found.");

            var factoryTypeInfos = types.Where(x => x.contractName == contractName);
            var result = new List<object>();

            foreach (var factoryTypeInfo in factoryTypeInfos)
                result.Add(GetInstance(factoryTypeInfo, parameters));

            return result;
        }

        private sealed class Component
        {
            public ComponentAttribute Attrib;
            public TypeInfo TypeInfo;
        }

        private class ObjectKey : DisposableBase
        {
            public FactoryTypeInfo FactoryTypeInfo { get; set; }

            public object Item { get; set; }

            public override string ToString()
            {
                return this.FactoryTypeInfo.contractName + " -> " + this.FactoryTypeInfo.type.FullName;
            }

            protected override void OnDispose(bool disposeManaged)
            {
                if (disposeManaged)
                    this.Item.TryDispose();
            }
        }
    }
}