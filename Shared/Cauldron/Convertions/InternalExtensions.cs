using System;
using System.Reflection;

namespace Cauldron
{
    internal static class InternalExtensions
    {
        public static object GetDefaultInstance(this Type type)
        {
#if WINDOWS_UWP || NETCORE
            var typeInfo = type.GetTypeInfo();

            // If no Type was supplied, if the Type was a reference type, or if the Type was a
            // System.Void, return null
            if (type == null || !typeInfo.IsValueType || type == typeof(void))
                return null;

            // If the supplied Type has generic parameters, its default value cannot be determined
            if (typeInfo.ContainsGenericParameters)
                throw new ArgumentException($"The supplied value type '{type}' contains generic parameters, so the default value cannot be retrieved");

            // If the Type is a primitive type, or if it is another publicly-visible value type (i.e.
            // struct/enum), return a default instance of the value type
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
            // If no Type was supplied, if the Type was a reference type, or if the Type was a
            // System.Void, return null
            if (type == null || !type.IsValueType || type == typeof(void))
                return null;

            // If the supplied Type has generic parameters, its default value cannot be determined
            if (type.ContainsGenericParameters)
                throw new ArgumentException($"The supplied value type '{type}' contains generic parameters, so the default value cannot be retrieved");

            // If the Type is a primitive type, or if it is another publicly-visible value type (i.e.
            // struct/enum), return a default instance of the value type
            if (type.IsPrimitive || !type.IsNotPublic)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch
                {
                    return null;
                }
            }
#endif
            return null;
        }

        public static bool IsNullable(this Type target)
        {
#if WINDOWS_UWP || NETCORE
            return target.GetTypeInfo().IsGenericType && Nullable.GetUnderlyingType(target) != null;
#else
            return target.IsGenericType && Nullable.GetUnderlyingType(target) != null;
#endif
        }
    }
}