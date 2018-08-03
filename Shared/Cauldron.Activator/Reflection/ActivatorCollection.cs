using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cauldron.Reflection
{
    internal class ActivatorCollection
    {
        private ConcurrentDictionary<string, ActivatorKey[]> activators = new ConcurrentDictionary<string, ActivatorKey[]>();

        private object syncObject = new object();

        public delegate object ObjectActivator(params object[] args);

        public object CreateInstance(ConstructorInfo ctor, object[] args)
        {
            ActivatorKey[] keys;
            var type = ctor.DeclaringType;

            if (activators.TryGetValue(type.FullName, out keys))
                return Activate(keys, args);

            return Activate(this.Add(type), args);
        }

        public object CreateInstance(Type type, object[] args)
        {
            ActivatorKey[] keys;

            if (activators.TryGetValue(type.FullName, out keys))
                return Activate(keys, args);

            return Activate(this.Add(type), args);
        }

        public object CreateInstance(Type type)
        {
            ActivatorKey[] keys;

            if (activators.TryGetValue(type.FullName, out keys))
                return Activate(keys, null);

            return Activate(this.Add(type), null);
        }

        public object CreateInstance(ConstructorInfo ctor)
        {
            ActivatorKey[] keys;
            var type = ctor.DeclaringType;

            if (activators.TryGetValue(type.FullName, out keys))
                return Activate(keys, null);

            return Activate(this.Add(type), null);
        }

        private static ObjectActivator GetActivator(ConstructorInfo ctor)
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

        private object Activate(ActivatorKey[] keys, object[] args)
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

        private ActivatorKey[] Add(Type type)
        {
            if (activators.ContainsKey(type.FullName))
                return activators[type.FullName];

            lock (syncObject)
            {
                if (activators.ContainsKey(type.FullName))
                    return activators[type.FullName];

                var ctors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).OrderBy(x => x.GetParameters().Length).ToArray();
                var activatorKeys = new ActivatorKey[ctors.Length];

                for (int i = 0; i < activatorKeys.Length; i++)
                    activatorKeys[i] = new ActivatorKey { parameterInfos = ctors[i].GetParameters(), activator = GetActivator(ctors[i]) };

                activators.TryAdd(type.FullName, activatorKeys);
                return activatorKeys;
            }
        }

        internal class ActivatorKey
        {
            public ObjectActivator activator;
            public ParameterInfo[] parameterInfos;
        }
    }
}