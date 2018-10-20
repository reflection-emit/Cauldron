using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cauldron.Reflection
{
    internal delegate object ObjectActivator(params object[] args);

    internal static class ExpressionActivator
    {
        internal static object Activate(ActivatorKey[] keys, object[] args)
        {
            if (keys.Length == 1)
                return keys[0].activator(args);

            if (args == null || args.Length == 0)
                return keys[0].activator();

            var types = args == null || args.Length == 0 ? Type.EmptyTypes : args.Select(x => x?.GetType() ?? null).ToArray();
            for (int i = 0; i < keys.Length; i++)
                if (keys[i].parameterInfos.MatchesArgumentTypes(types))
                    return keys[i].activator(args);

            throw new MissingMethodException($"A constructor with the given arguments was not found.");
        }

        internal static ObjectActivator GetActivator(ConstructorInfo ctor)
        {
            // from https://rogeralsing.com/2008/02/28/linq-expressions-creating-objects/
            var paramsInfo = ctor.GetParameters();

            // create a single param of type object[]
            var param = Expression.Parameter(typeof(object[]), "args");
            var argsExp = new Expression[paramsInfo.Length];

            // pick each arg from the params array  and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                var index = Expression.Constant(i);
                var paramType = paramsInfo[i].ParameterType;
                var paramAccessorExp = Expression.ArrayIndex(param, index);
                var paramCastExp = Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            // make a NewExpression that calls the ctor with the args we just created
            var newExp = Expression.New(ctor, argsExp);

            // create a lambda with the New Expression as body and our param object[] as arg
            var lambda = Expression.Lambda(typeof(ObjectActivator), newExp, param);

            // compile it
            return lambda.Compile() as ObjectActivator;
        }
    }
}