using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cauldron.Core
{
    internal static class IEquatableCache
    {
        private static ConcurrentDictionary<string, EqualsMethod> cache = new ConcurrentDictionary<string, EqualsMethod>();

        public delegate bool EqualsMethod(object instance, object other);

        public static EqualsMethod Get(Type @this, Type other)
        {
            var key = @this.FullName + other.FullName;

            if (cache.TryGetValue(key, out EqualsMethod result))
                return result;

            // We have to do it this way to avoid reference assignables...
            var method = @this.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(x => x.Name == "Equals" && x.GetParameters() is ParameterInfo[] @params && @params.Length > 0 && @params[0].ParameterType == other);

            if (method == null)
                return null;

            var param = Expression.Parameter(typeof(object), "param");
            var argsExp = Expression.Convert(param, other);
            var instance = Expression.Parameter(typeof(object), "instance");
            var instanceExp = Expression.Convert(instance, @this);
            var callExp = Expression.Call(instanceExp, method, argsExp);
            var lambda = Expression.Lambda(typeof(EqualsMethod), callExp, instance, param);

            result = lambda.Compile() as EqualsMethod;
            cache.TryAdd(key, result);

            return result;
        }
    }
}