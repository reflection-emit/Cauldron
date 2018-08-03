using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cauldron.Convertions
{
    internal static class ImplicitExplicitConvertionCache
    {
        private static ConcurrentDictionary<string, ImplicitExplicit> cache = new ConcurrentDictionary<string, ImplicitExplicit>();

        public delegate object ImplicitExplicit(object source);

        public static ImplicitExplicit Get(Type source, Type target)
        {
            var key = source.FullName + target.FullName;

            if (cache.TryGetValue(key, out ImplicitExplicit result))
                return result;

            var method = target.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(x => (x.Name == "op_Implicit" || x.Name == "op_Explicit") && x.ReturnType == target && x.GetParameters()[0].ParameterType == source);

            if (method == null)
                method = source.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(x => (x.Name == "op_Implicit" || x.Name == "op_Explicit") && x.ReturnType == target && x.GetParameters()[0].ParameterType == source);

            if (method == null)
                return null;

            var param = Expression.Parameter(typeof(object), "param");
            var argsExp = Expression.Convert(param, source);
            var callExp = Expression.Call(method, argsExp);
            var lambda = Expression.Lambda(typeof(ImplicitExplicit), callExp, param);

            result = lambda.Compile() as ImplicitExplicit;
            cache.TryAdd(key, result);

            return result;
        }
    }
}