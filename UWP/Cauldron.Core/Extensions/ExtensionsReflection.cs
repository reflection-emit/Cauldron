using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cauldron.Core.Extensions
{
    /// <summary>
    /// Provides a set of extensions used in reflection
    /// </summary>
    public static class ExtensionsReflection
    {
        private static ConcurrentDictionary<ConstructorInfo, ObjectActivator> objectActivator = new ConcurrentDictionary<ConstructorInfo, ObjectActivator>();

        private delegate object ObjectActivator(params object[] args);

        /// <summary>
        /// Returns a value that indicates whether the specified type can be assigned to the current type.
        /// </summary>
        /// <param name="type">The current type that will be assigned to</param>
        /// <param name="toBeAssigned">The type to check.</param>
        /// <returns>true if the specified type can be assigned to this type; otherwise, false.</returns>
        public static bool AreReferenceAssignable(this Type type, Type toBeAssigned)
        {
#if WINDOWS_UWP
            var typeInfo = type.GetTypeInfo();
            var toBeAssignedTypeInfo = toBeAssigned.GetTypeInfo();
            if (type == toBeAssigned || (!typeInfo.IsValueType && !toBeAssignedTypeInfo.IsValueType && typeInfo.IsAssignableFrom(toBeAssignedTypeInfo)) || (typeInfo.IsInterface && toBeAssigned == typeof(object)))
#else

            if (type == toBeAssigned || (!type.IsValueType && !toBeAssigned.IsValueType && type.IsAssignableFrom(toBeAssigned)) || (type.IsInterface && toBeAssigned == typeof(object)))
#endif
                return true;

            return false;
        }

        /// <summary>
        /// Creates an instance of the specified type using the constructor that best matches the specified parameters.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        /// <param name="args">
        /// An array of arguments that match in number, order, and type the parameters of
        /// the constructor to invoke. If args is an empty array or null, the constructor
        /// that takes no parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A reference to the newly created object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null</exception>
        /// <exception cref="CreateInstanceIsAnInterfaceException"><paramref name="type"/> is an interface</exception>
        public static object CreateInstance(this Type type, params object[] args)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsInterface)
                throw new CreateInstanceIsAnInterfaceException("Unable to create an instance from an interface: " + type.FullName);

            var types = args == null || args.Length == 0 ? Type.EmptyTypes : args.Select(x => x == null ? typeof(object) : x.GetType()).ToArray();
            var ctor = type.GetConstructor(types);

            if (ctor == null)
                ctor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(x => x.MatchesArgumentTypes(types));

            if (ctor == null)
                ctor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(x => x.GetParameters().Length == types.Length);

            if (ctor == null)
                throw new MissingMethodException($"A constructor with the given arguments was not found in type '{type.FullName}'");

            return ctor.CreateInstance(args);
        }

        /// <summary>
        /// Creates an instance of the specified type using the constructor
        /// </summary>
        /// <param name="ctor">The constructor use to construct the instance</param>
        /// <param name="args">
        /// An array of arguments that match in number, order, and type the parameters of
        /// the constructor to invoke. If args is an empty array or null, the constructor
        /// that takes no parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A reference to the newly created object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="ctor"/> is null</exception>
        public static object CreateInstance(this ConstructorInfo ctor, object[] args)
        {
            if (ctor == null)
                throw new ArgumentNullException(nameof(ctor));

            if (objectActivator.ContainsKey(ctor))
                return objectActivator[ctor](args != null && args.Length == 0 ? null : args);
            else
            {
                var createdActivator = GetActivator(ctor);
                objectActivator.TryAdd(ctor, createdActivator);

                return createdActivator(args != null && args.Length == 0 ? null : args);
            }
        }

        /// <summary>
        /// Returns the type of T in an <see cref="IEnumerable{T}"/> implementation
        /// </summary>
        /// <param name="type">The <see cref="Type"/> with the <see cref="IEnumerable{T}"/> implementation</param>
        /// <returns>The type of T in an <see cref="IEnumerable{T}"/> implementation</returns>
        public static Type GetChildrenType(this Type type)
        {
            if (type.IsArray)
                return type.GetElementType();

#if WINDOWS_UWP
            var interfaceType = type.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
#else
            var interfaceType = type.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
#endif
            if (interfaceType == null)
                return typeof(object);

            return interfaceType.GetGenericArguments()[0];
        }

        /// <summary>
        /// Retrieves the default value for a given Type.
        /// <para/>
        /// http://stackoverflow.com/questions/2490244/default-value-of-a-type-at-runtime
        /// By Mark Jones
        /// </summary>
        /// <param name="type">The Type for which to get the default value</param>
        /// <returns>The default value for <paramref name="type"/></returns>
        /// <remarks>
        /// If a null Type, a reference Type, or a System.Void Type is supplied, this method always returns null.  If a value type
        /// is supplied which is not publicly visible or which contains generic parameters, this method will fail with an
        /// exception.
        /// </remarks>
        public static object GetDefaultInstance(this Type type)
        {
#if WINDOWS_UWP
            var typeInfo = type.GetTypeInfo();

            // If no Type was supplied, if the Type was a reference type, or if the Type was a System.Void, return null
            if (type == null || !typeInfo.IsValueType || type == typeof(void))
                return null;

            // If the supplied Type has generic parameters, its default value cannot be determined
            if (typeInfo.ContainsGenericParameters)
                throw new ArgumentException($"The supplied value type '{type}' contains generic parameters, so the default value cannot be retrieved");

            // If the Type is a primitive type, or if it is another publicly-visible value type (i.e. struct/enum), return a
            // default instance of the value type
            if (typeInfo.IsPrimitive || !typeInfo.IsNotPublic)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"The Activator.CreateInstance method could not create a default instance of the supplied value type '{type}'", e);
                }
            }

#else
            // If no Type was supplied, if the Type was a reference type, or if the Type was a System.Void, return null
            if (type == null || !type.IsValueType || type == typeof(void))
                return null;

            // If the supplied Type has generic parameters, its default value cannot be determined
            if (type.ContainsGenericParameters)
                throw new ArgumentException($"The supplied value type '{type}' contains generic parameters, so the default value cannot be retrieved");

            // If the Type is a primitive type, or if it is another publicly-visible value type (i.e. struct/enum), return a
            // default instance of the value type
            if (type.IsPrimitive || !type.IsNotPublic)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"The Activator.CreateInstance method could not create a default instance of the supplied value type '{type}'", e);
                }
            }
#endif

            // Fail with exception
            throw new ArgumentException($"The supplied value type '{type}' is not a publicly-visible type, so the default value cannot be retrieved");
        }

        /// <summary>
        /// Returns the type of TKey and TValue in an <see cref="IDictionary{TKey, TValue}"/> implementation
        /// </summary>
        /// <param name="type">The <see cref="Type"/> with the <see cref="IDictionary{TKey, TValue}"/> implementation</param>
        /// <returns>The type of TKey and TValue in an <see cref="IDictionary{TKey, TValue}"/> implementation</returns>
        public static Type[] GetDictionaryKeyValueTypes(this Type type)
        {
#if WINDOWS_UWP
            var interfaceType = type.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));
#else
            var interfaceType = type.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));
#endif
            if (interfaceType == null)
                return null;

            return interfaceType.GetGenericArguments();
        }

        /// <summary>
        /// Returns all the fields of the current Type, using the specified binding constraints including those of the base classes.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that contains the fields</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that specify how the search is conducted.</param>
        /// <returns>A collection of <see cref="FieldInfo"/> objects representing all the fields defined for the current Type.</returns>
        public static IEnumerable<FieldInfo> GetFieldsEx(this Type type, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
            var fields = type.GetFields(bindingFlags | BindingFlags.DeclaredOnly);

#if WINDOWS_UWP
            var t = type.GetTypeInfo();
#else
            var t = type;

#endif
            if (t.BaseType == null)
                return fields;

            return fields.Union(t.BaseType.GetFieldsEx(bindingFlags));
        }

        /// <summary>
        /// Returns all the methods of the current Type, using the specified binding constraints including those of the base classes.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that contains the method</param>
        /// <param name="methodName">The name of the method to get</param>
        /// <param name="parameterTypes">The type of parameters the method should have</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that specify how the search is conducted.</param>
        /// <returns>A <see cref="MethodInfo"/> object representing the method defined for the current Type that match the specified binding constraint.</returns>
        public static MethodInfo GetMethod(this Type type, string methodName, Type[] parameterTypes, BindingFlags bindingFlags)
        {
#if WINDOWS_UWP

            return type.GetMethods(bindingFlags).FirstOrDefault(x => x.Name == methodName && x.MatchesArgumentTypes(parameterTypes));
#else
            return type.GetMethod(methodName, bindingFlags, null, parameterTypes, null);

#endif
        }

        /// <summary>
        /// Searches for the specified public method whose parameters match the specified argument types.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that contains the methods</param>
        /// <param name="methodName">The string containing the name of the method to get. </param>
        /// <param name="parameterTypes">The type of parameters the method should have</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that specify how the search is conducted.</param>
        /// <returns>An object representing the public method whose parameters match the specified argument types, if found; otherwise, null.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="methodName"/> is null</exception>
        public static MethodInfo GetMethodEx(this Type type, string methodName, Type[] parameterTypes, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (methodName == null)
                throw new ArgumentNullException(nameof(methodName));

#if WINDOWS_UWP
            if (!type.GetTypeInfo().IsInterface)
#else
            if (!type.IsInterface)
#endif
                return type.GetMethod(methodName, parameterTypes, bindingFlags);

            return type
            .GetInterfaces()
            .Concat(new Type[] { type })
            .Select(x => x.GetMethod(methodName, parameterTypes, bindingFlags | BindingFlags.FlattenHierarchy | BindingFlags.DeclaredOnly))
            .FirstOrDefault(x => x != null);
        }

        /// <summary>
        /// Returns all the methods of the current Type, using the specified binding constraints including those of the base classes.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that contains the methods</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that specify how the search is conducted.</param>
        /// <returns>A collection of <see cref="MethodInfo"/> objects representing all the methods defined for the current Type.</returns>
        public static IEnumerable<MethodInfo> GetMethodsEx(this Type type, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
#if WINDOWS_UWP

            if (!type.GetTypeInfo().IsInterface)
#else
            if (!type.IsInterface)
#endif
            {
                var methods = type.GetMethods(bindingFlags | BindingFlags.DeclaredOnly);
#if WINDOWS_UWP
                var baseType = type.GetTypeInfo().BaseType;
#else
                var baseType = type.BaseType;
#endif

                if (baseType == null)
                    return methods;

                return methods.Union(baseType.GetMethodsEx(bindingFlags));
            }

            return type.GetInterfaces().Concat(new Type[] { type }).SelectMany(x => x.GetMethods(bindingFlags | BindingFlags.FlattenHierarchy | BindingFlags.DeclaredOnly));
        }

        /// <summary>
        /// Returns all the properties of the current Type, using the specified binding constraints including those of the base classes.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that contains the properties</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that specify how the search is conducted.</param>
        /// <returns>A collection of <see cref="PropertyInfo"/> objects representing all the properties defined for the current Type.</returns>
        public static IEnumerable<PropertyInfo> GetPropertiesEx(this Type type, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
#if WINDOWS_UWP
            if (!type.GetTypeInfo().IsInterface)
#else
            if (!type.IsInterface)
#endif
            {
                var properties = type.GetProperties(bindingFlags | BindingFlags.DeclaredOnly);

#if WINDOWS_UWP
                var baseType = type.GetTypeInfo().BaseType;
#else
                var baseType = type.BaseType;
#endif

                if (baseType == null)
                    return properties;

                return properties.Union(baseType.GetPropertiesEx(bindingFlags));
            }

            return type.GetInterfaces().Concat(new Type[] { type }).SelectMany(x => x.GetProperties(bindingFlags | BindingFlags.FlattenHierarchy | BindingFlags.DeclaredOnly));
        }

        /// <summary>
        /// Gets a specific property of the current Type.
        /// <para/>
        /// This method will try to find the exact property if an <see cref="AmbiguousMatchException"/> occures
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that contains the property</param>
        /// <param name="propertyName">The string containing the name of the property to get</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that specify how the search is conducted.</param>
        /// <returns>An object representing the public property with the specified name, if found; otherwise, null.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is null</exception>
        public static PropertyInfo GetPropertyEx(this Type type, string propertyName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));
#if WINDOWS_UWP
            if (!type.GetTypeInfo().IsInterface)
#else

            if (!type.IsInterface)
#endif
            {
                try
                {
                    return type.GetProperty(propertyName, bindingFlags);
                }
                catch (AmbiguousMatchException)
                {
                    return type.GetProperty(propertyName, bindingFlags | BindingFlags.DeclaredOnly);
                }
            }

            return type
                .GetInterfaces()
                .Concat(new Type[] { type })
                .Select(x => x.GetProperty(propertyName, bindingFlags | BindingFlags.FlattenHierarchy | BindingFlags.DeclaredOnly))
                .FirstOrDefault(x => x != null);
        }

        /// <summary>
        /// Tries to find a property defined by a path
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that contains the property</param>
        /// <param name="path">The path of the property</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that specify how the search is conducted.</param>
        /// <returns>An object representing the public property with the specified name, if found; otherwise, null.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        public static PropertyInfo GetPropertyFromPath(this Type type, string path, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var bindingPath = path.Split('.');
            PropertyInfo result = null;

            // let us follow the path and change the source accordingly
            for (int i = 0; i < bindingPath.Length; i++)
            {
                if (type == null)
                    break;

                var section = bindingPath[i];

                result = type.GetPropertyEx(section);

                if (result == null)
                    // the path is invalid...
                    return null;

                type = result.PropertyType;
            }

            return result;
        }

        /// <summary>
        /// Searches for the specified property, using the specified binding constraints and returns its value.
        /// <para/>
        /// Default <see cref="BindingFlags"/> are <see cref="BindingFlags.Instance"/> and <see cref="BindingFlags.NonPublic"/>
        /// </summary>
        /// <typeparam name="T">The property's return value type</typeparam>
        /// <param name="obj">The <see cref="object"/> to retrieve the value from</param>
        /// <param name="propertyName">The string containing the name of the property to get. </param>
        /// <returns>The property value of the specified object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is null</exception>
        /// <exception cref="NullReferenceException">The property defined by <paramref name="propertyName"/> was not found</exception>
        public static T GetPropertyNonPublicValue<T>(this object obj, string propertyName) =>
            obj.GetPropertyValue<T>(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Searches for the specified property, using the specified binding constraints and returns its value.
        /// </summary>
        /// <typeparam name="T">The property's return value type</typeparam>
        /// <param name="obj">The <see cref="object"/> to retrieve the value from</param>
        /// <param name="propertyName">The string containing the name of the property to get. </param>
        /// <param name="bindingFlags">
        /// A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the search is conducted.
        /// <para/>
        /// Zero, to return null.
        /// </param>
        /// <returns>The property value of the specified object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is null</exception>
        /// <exception cref="NullReferenceException">The property defined by <paramref name="propertyName"/> was not found</exception>
        public static T GetPropertyValue<T>(this object obj, string propertyName, BindingFlags bindingFlags) =>
            (T)obj.GetPropertyValue(propertyName, bindingFlags);

        /// <summary>
        /// Searches for the specified property, using the specified binding constraints and returns its value.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to retrieve the value from</param>
        /// <param name="propertyName">The string containing the name of the property to get. </param>
        /// <param name="bindingFlags">
        /// A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the search is conducted.
        /// <para/>
        /// Zero, to return null.
        /// </param>
        /// <returns>The property value of the specified object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is null</exception>
        /// <exception cref="NullReferenceException">The property defined by <paramref name="propertyName"/> was not found</exception>
        public static object GetPropertyValue(this object obj, string propertyName, BindingFlags bindingFlags)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var propertyInfo = obj.GetType().GetPropertyEx(propertyName, bindingFlags);

            if (propertyInfo == null)
                throw new NullReferenceException("The property '{0}' was not found on type '{1}'".ToString(propertyName, obj.GetType().FullName));

            return propertyInfo.GetValue(obj);
        }

        /// <summary>
        /// Searches for the specified property, using the specified binding constraints and returns its value.
        /// <para/>
        /// Default <see cref="BindingFlags"/> are <see cref="BindingFlags.Instance"/> and <see cref="BindingFlags.Public"/>
        /// </summary>
        /// <typeparam name="T">The property's return value type</typeparam>
        /// <param name="obj">The <see cref="object"/> to retrieve the value from</param>
        /// <param name="propertyName">The string containing the name of the property to get. </param>
        /// <returns>The property value of the specified object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is null</exception>
        /// <exception cref="NullReferenceException">The property defined by <paramref name="propertyName"/> was not found</exception>
        public static T GetPropertyValue<T>(this object obj, string propertyName) =>
            obj.GetPropertyValue<T>(propertyName, BindingFlags.Instance | BindingFlags.Public);

        /// <summary>
        /// Checks if the type has implemented the defined interface
        /// </summary>
        /// <typeparam name="T">The type of interface to look for</typeparam>
        /// <param name="type">The type that may implements the interface <typeparamref name="T"/></param>
        /// <exception cref="ArgumentException">The type <typeparamref name="T"/> is not an interface</exception>
        /// <returns>True if the <paramref name="type"/> has implemented the interface <typeparamref name="T"/></returns>
        public static bool ImplementsInterface<T>(this Type type) =>
            type.GetTypeInfo().ImplementsInterface<T>();

        /// <summary>
        /// Checks if the type has implemented the defined interface
        /// </summary>
        /// <param name="type">The type that may implements the interface</param>
        /// <param name="typeOfInterface">The type of interface to look for</param>
        /// <exception cref="ArgumentException">The type <paramref name="typeOfInterface"/> is not an interface</exception>
        /// <returns>True if the <paramref name="type"/> has implemented the interface <paramref name="typeOfInterface"/></returns>
        public static bool ImplementsInterface(this Type type, Type typeOfInterface) =>
            type.GetTypeInfo().ImplementsInterface(typeOfInterface);

        /// <summary>
        /// Checks if the type has implemented the defined interface
        /// </summary>
        /// <typeparam name="T">The type of interface to look for</typeparam>
        /// <param name="typeInfo">The type that may implements the interface <typeparamref name="T"/></param>
        /// <exception cref="ArgumentException">The type <typeparamref name="T"/> is not an interface</exception>
        /// <returns>True if the <paramref name="typeInfo"/> has implemented the interface <typeparamref name="T"/></returns>
        public static bool ImplementsInterface<T>(this TypeInfo typeInfo) =>
            typeInfo.ImplementsInterface(typeof(T));

        /// <summary>
        /// Checks if the type has implemented the defined interface
        /// </summary>
        /// <param name="typeInfo">The type that may implements the interface <paramref name="typeOfInterface"/></param>
        /// <param name="typeOfInterface">The type of interface to look for</param>
        /// <exception cref="ArgumentException">The type <paramref name="typeOfInterface"/> is not an interface</exception>
        /// <returns>True if the <paramref name="typeInfo"/> has implemented the interface <paramref name="typeOfInterface"/></returns>
        public static bool ImplementsInterface(this TypeInfo typeInfo, Type typeOfInterface)
        {
#if WINDOWS_UWP
            var typeOfInterfaceInfo = typeOfInterface.GetTypeInfo();

            if (!typeOfInterfaceInfo.IsInterface)
                throw new ArgumentException("T is not an interface", nameof(typeOfInterface));

            var interfaces = typeInfo.ImplementedInterfaces;

            if (typeOfInterfaceInfo.IsGenericTypeDefinition)
                return interfaces.Any(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeOfInterface);
            else
                return interfaces.Any(x => x == typeOfInterface);
#else

            if (!typeOfInterface.IsInterface)
                throw new ArgumentException("T is not an interface", nameof(typeOfInterface));

            var interfaces = typeInfo.ImplementedInterfaces;

            if (typeOfInterface.IsGenericTypeDefinition)
                return interfaces.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeOfInterface);
            else
                return interfaces.Any(x => x == typeOfInterface);
#endif
        }

        /// <summary>
        /// Checks if the type implements the <see cref="ICollection"/> or <see cref="IList"/> interface
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>Returns true if the type is a collection or a list; otherwise false</returns>
        public static bool IsCollectionOrList(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetTypeInfo().ImplementedInterfaces.Any(x => x == typeof(ICollection) || x == typeof(IList));
        }

        /// <summary>
        /// Gets a value indicating whether the current type is a <see cref="Nullable{T}"/>
        /// </summary>
        /// <param name="target">The type to test</param>
        /// <returns>Returns true if the type is <see cref="Nullable{T}"/></returns>
        public static bool IsNullable(this Type target)
        {
#if WINDOWS_UWP
            return target.GetTypeInfo().IsGenericType && Nullable.GetUnderlyingType(target) != null;
#else
            return target.IsGenericType && Nullable.GetUnderlyingType(target) != null;
#endif
        }

        /// <summary>
        /// Returns true if the argument types defined by <paramref name="argumentTypes"/> matches with the argument types of <paramref name="method"/>
        /// </summary>
        /// <param name="method">The method info which has to be compared to</param>
        /// <param name="argumentTypes">The argument types that has to match to</param>
        /// <returns>true if the argument types of <paramref name="method"/> matches with the argument type defined by <paramref name="argumentTypes"/>; otherwise, false.</returns>
        public static bool MatchesArgumentTypes(this MethodBase method, Type[] argumentTypes)
        {
            if (method == null || argumentTypes == null)
                return false;

            var parameters = method.GetParameters();

            if (parameters.Length == argumentTypes.Length)
            {
                for (int i = 0; i < parameters.Length; i++)
                    if (!parameters[i].ParameterType.AreReferenceAssignable(argumentTypes[i]))
                        return false;

                return true;
            }

            return false;
        }

        private static ObjectActivator GetActivator(ConstructorInfo ctor)
        {
            // from https://rogeralsing.com/2008/02/28/linq-expressions-creating-objects/

            Type type = ctor.DeclaringType;
            var paramsInfo = ctor.GetParameters();

            // create a single param of type object[]
            var param = Expression.Parameter(typeof(object[]), "args");
            var argsExp = new Expression[paramsInfo.Length];

            // pick each arg from the params array 
            // and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                var index = Expression.Constant(i);
                var paramType = paramsInfo[i].ParameterType;
                var paramAccessorExp = Expression.ArrayIndex(param, index);
                var paramCastExp = Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            // make a NewExpression that calls the
            // ctor with the args we just created
            var newExp = Expression.New(ctor, argsExp);

            // create a lambda with the New
            // Expression as body and our param object[] as arg
            var lambda = Expression.Lambda(typeof(ObjectActivator), newExp, param);

            // compile it
            return lambda.Compile() as ObjectActivator;
        }
    }
}