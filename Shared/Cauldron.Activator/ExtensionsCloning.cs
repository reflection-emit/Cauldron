using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cauldron.Activator
{
    /// <summary>
    /// Provides usefull extension methods regarding cloning and mapping
    /// </summary>
    public static class ExtensionsCloning
    {
        /// <summary>
        /// Uses <see cref="ExtensionsCloning.MapTo{T, TTarget}(T, TTarget)"/> to clone an object instance to an object with the same type
        /// </summary>
        /// <typeparam name="T">The type of the source</typeparam>
        /// <param name="source">The source object</param>
        /// <returns>Returns the clone of <paramref name="source"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null</exception>
        [Obsolete("Use Mapster instead")]
        public static T DeepClone<T>(this T source) where T : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return CopyObject(source.GetType(), source) as T;
        }

        /// <summary>
        /// Maps all properties and fields of an instance to another instance. The Clone() method is used to copy an instance if exist.
        /// <para/>
        /// Mapping fails on jagged and multidimensional array. Classes without parameterless constructor will stay null.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the source instance</typeparam>
        /// <typeparam name="TTarget">The <see cref="Type"/> of the target instance</typeparam>
        /// <param name="source">The source object</param>
        /// <param name="target">The target object</param>
        /// <returns>Returns the target</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is null</exception>
        [Obsolete("Use Mapster instead")]
        public static TTarget MapTo<T, TTarget>(this T source, TTarget target)
        {
            /*
                This is a very long Method and should stay this way due to performance reasons
            */

            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            object result = target; // so that we can map value types also
            var targetType = result.GetType();

            #region Map Properties

            var targetProperties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.CanWrite && x.SetMethod.IsPublic);

            foreach (var property in source.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanRead && targetProperties.Any<PropertyInfo>(y => y.Name == x.Name))
                .Where(x => x.GetCustomAttribute<CloneIgnoreAttribute>() == null))
            {
                var targetPropertyInfo = targetProperties.FirstOrDefault(x => x.Name == property.Name);
                var value = property.GetValue(source);

                // if the value is null, just assign it directly if its nullable or it is an object
#if WINDOWS_UWP || NETCORE
                if (value == null && !property.PropertyType.GetTypeInfo().IsPrimitive)
#else
                if (value == null && !property.PropertyType.IsPrimitive)
#endif
                {
                    targetPropertyInfo.SetValue(result, null);
                    continue;
                }

                if (value == null || Object.ReferenceEquals(value, target))
                    continue;

                // Do not use the PropertyType, because the value
                // could contain an object that inherits from PropertyType
                // and has its own Method and Property implementations
                var valueType = value.GetType();
                targetPropertyInfo.SetValue(result, CopyObject(valueType, value));
            }

            #endregion Map Properties

            #region Map Fields

            var targetFields = result.GetType().GetFieldsEx(BindingFlags.Public | BindingFlags.Instance).Where(x => !x.IsInitOnly /* Read only fields */);

            foreach (var field in source.GetType()
                .GetFieldsEx(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => targetFields.Any<FieldInfo>(y => y.Name == x.Name))
                .Where(x => x.GetCustomAttribute<CloneIgnoreAttribute>() == null))
            {
                var targetFieldInfo = targetFields.FirstOrDefault(x => x.Name == field.Name);
                var value = field.GetValue(source);

                // if the value is null, just assign it directly if its nullable or it is an object
#if WINDOWS_UWP || NETCORE
                if (value == null && !field.FieldType.GetTypeInfo().IsPrimitive)
#else
                if (value == null && !field.FieldType.IsPrimitive)
#endif
                {
                    targetFieldInfo.SetValue(result, null);
                    continue;
                }

                // do nothing if the value is null
                if (value == null || Object.ReferenceEquals(value, target))
                    continue;

                // Do not use the FieldType, because the value
                // could contain an object that inherits from FieldType
                // and has its own Method and Property implementations
                var valueType = value.GetType();
                targetFieldInfo.SetValue(result, CopyObject(valueType, value));
            }

            #endregion Map Fields

            return (TTarget)result;
        }

        /// <summary>
        /// Maps all properties and fields of an instance to another instance. The Clone() method is used to copy an instance if exist.
        /// <para/>
        /// Mapping fails on jagged and multidimensional array. Classes without parameterless constructor will stay null.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the target instance</typeparam>
        /// <param name="source">The source object</param>
        /// <returns>Returns the target</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null</exception>
        [Obsolete("Use Mapster instead")]
        public static T MapTo<T>(this object source) where T : class, new() =>
            source.MapTo(Factory.Create<T>(callingType: typeof(T)));

        private static object CopyObject(Type valueType, object value)
        {
            if (valueType == null)
                return null;

            object result = null;

            // Primitive type and value type and nullables are just simply copied
#if WINDOWS_UWP || NETCORE
            var typeInfo = valueType.GetTypeInfo();
            if (typeInfo.IsPrimitive || typeInfo.IsValueType || valueType.IsNullable() || (typeInfo.IsAbstract && typeInfo.IsSealed) /* These are static values*/)
#else
            if (valueType.IsPrimitive || valueType.IsValueType || valueType.IsNullable() || (valueType.IsAbstract && valueType.IsSealed) /* These are static values*/)
#endif
                return value;

            // special case for strings
            if (valueType == typeof(string))
                return (value as string).Copy();

            if (!valueType.IsArray) // Dont do this with array... Array Clone returns the same reference types
            {
#if !WINDOWS_UWP
                // let us check first if the clonable interface is implemented... Casting is faster than reflection so let us prefer this
                var clonableInterface = value as ICloneable;

                if (clonableInterface != null)
                    return clonableInterface.Clone();
#endif

                // We should check if the value has a Clone method... If this is the.. Use it
                var cloneMethodInfo = valueType.GetMethod("Clone", new Type[] { }, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

                // we should only use this method if the value inherits from return value or is the same
                if (cloneMethodInfo != null && cloneMethodInfo.ReturnType.IsAssignableFrom(valueType))
                    return cloneMethodInfo.Invoke(value, null);
            }

            // We also try to copy IEnumerables
            if (valueType.IsArray || typeof(IEnumerable).IsAssignableFrom(valueType))
            {
                if (valueType.IsArray)
                    return CreateArray(valueType.GetElementType(), value);

                var childType = valueType.GetChildrenType();
                result = valueType.CreateInstance();

                // Check if this collection has a addrange
                var addRange = valueType.GetMethod("AddRange", new Type[] { typeof(IEnumerable<>).MakeGenericType(childType) }, BindingFlags.Instance | BindingFlags.Public);

                if (addRange != null)
                    addRange.Invoke(result, new object[] { CreateArray(childType, value) });
                else
                {
                    // If not... Use the add method
                    var addMethod = valueType.GetMethod("Add", new Type[] { childType }, BindingFlags.Instance | BindingFlags.Public);
                    if (addMethod != null)
                        foreach (var item in value as IEnumerable)
                            addMethod.Invoke(result, new object[] { CopyObject(item.GetType(), item) });
                    // this could be a dictionary
                    else if (valueType.ImplementsInterface(typeof(IDictionary<,>)))
                    {
                        var dictionaryTypes = valueType.GetDictionaryKeyValueTypes();
                        if (dictionaryTypes == null)
                            return null; // we cant clone this

                        addMethod = valueType.GetMethod("Add", dictionaryTypes, BindingFlags.Instance | BindingFlags.Public);
                        if (addMethod == null)
                            return null;

                        // The dictionary does not garantee the same order everytime you iterate through it
                        foreach (var item in value as IEnumerable)
                        {
                            var key = item.GetPropertyValue("Key", BindingFlags.Instance | BindingFlags.Public);
                            var val = item.GetPropertyValue("Value", BindingFlags.Instance | BindingFlags.Public);
                            addMethod.Invoke(result, new object[]
                            {
                                CopyObject(key.GetType(), key),
                                CopyObject(val?.GetType(), val)
                            });
                        }
                    }
                    else
                        return null; // We cant clone this
                }

                return result;
            }

#if WINDOWS_UWP || NETCORE
            // If this is a struct or a class with parameterless constructor we just create a value directly
            else if (typeInfo.IsValueType && !typeInfo.IsEnum || valueType.GetConstructor(Type.EmptyTypes) != null)
#else
            else if (valueType.IsValueType && !valueType.IsEnum || valueType.GetConstructor(Type.EmptyTypes) != null)
#endif
            {
                result = Factory.Create(valueType, callingType: valueType);

                if (result == null)
                    result = valueType.CreateInstance();
            }
#if WINDOWS_UWP || NETCORE
            else if (typeInfo.IsSealed /* Values that does not have a public constructor and also sealed can only be created by specialized static methods. In such cases we dont try to clone the value... */ )
#else
            else if (valueType.IsSealed /* Values that does not have a public constructor and also sealed can only be created by specialized static methods. In such cases we dont try to clone the value... */ )
#endif
                result = value;
            else
                // Don't copy anything if the object has no parameterless constructor
                return null;

            return value.MapTo(result);
        }

        private static int Count(IEnumerable enumerable)
        {
            var source = enumerable as ICollection;
            if (source != null)
                return source.Count;

            var enumerator = enumerable.GetEnumerator();
            try
            {
                int count = 0;
                while (enumerator.MoveNext())
                    count++;
                return count;
            }
            catch
            {
                return 0;
            }
            finally
            {
                enumerator.TryDispose();
            }
        }

        private static IEnumerable CreateArray(Type valueType, object value)
        {
            var items = value as IEnumerable;
            var count = Count(items);
            int counter = 0;

            var array = Array.CreateInstance(valueType, count);
            foreach (var item in items)
                if (item != null)
                    array.SetValue(CopyObject(item.GetType(), item), counter++);

            return array;
        }
    }
}