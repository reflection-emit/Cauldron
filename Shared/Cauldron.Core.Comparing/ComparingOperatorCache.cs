using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cauldron.Core
{
    internal static class ComparingOperatorCache
    {
        private static ConcurrentDictionary<string, ComparerOperator> cache = new ConcurrentDictionary<string, ComparerOperator>();

        public delegate bool ComparerOperator(object a, object b);

        public enum Operator : byte
        {
            GreaterThan,
            GreaterThanOrEqual,
            LessThan,
            LessThanOrEqual,
            Equal,
            Inequality
        }

        public static ComparerOperator Get(Operator @operator, Type a, Type b)
        {
            string operatorName;
            switch (@operator)
            {
                case Operator.GreaterThan: operatorName = "op_GreaterThan"; break;
                case Operator.GreaterThanOrEqual: operatorName = "op_GreaterThanOrEqual"; break;
                case Operator.LessThan: operatorName = "op_LessThan"; break;
                case Operator.LessThanOrEqual: operatorName = "op_LessThanOrEqual"; break;
                case Operator.Equal: operatorName = "op_Equality"; break;
                case Operator.Inequality: operatorName = "op_Inequality"; break;
                default: throw new ArgumentException("Unknown operator type");
            }

            var key = a.FullName + b.FullName + operatorName;

            if (cache.TryGetValue(key, out ComparerOperator result))
                return result;

            // We have to do it this way to avoid reference assignables...
            var parameters = new Type[] { a, b };
            var method = a.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(x => x.Name == operatorName && x.GetParameters().Select(y => y.ParameterType).SequenceEqual(parameters));

            if (method == null)
                return null;

            var paramA = Expression.Parameter(typeof(object), "paramA");
            var paramB = Expression.Parameter(typeof(object), "paramB");
            var argsExpA = Expression.Convert(paramA, a);
            var argsExpB = Expression.Convert(paramB, b);
            var callExp = Expression.Call(method, argsExpA, argsExpB);
            var lambda = Expression.Lambda(typeof(ComparerOperator), callExp, paramA, paramB);

            result = lambda.Compile() as ComparerOperator;
            cache.TryAdd(key, result);

            return result;
        }
    }
}