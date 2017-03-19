using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cauldron.Core.Extensions
{
    internal class ActivatorArray
    {
        /*
            Attention: This class is optimized for performance; therefor can be messy and ununderstandable... Sorry
        */

        private ObjectActivator[] activators = new ObjectActivator[30];
        private ActivatorArrayKey[] keys = new ActivatorArrayKey[30];
        private volatile int length = 0;
        private object syncObject = new object();

        public delegate object ObjectActivator(params object[] args);

        public object CreateInstance(ConstructorInfo ctor, object[] args)
        {
            var parameters = ctor.GetParameters();
            ObjectActivator activator = null;
            var type = ctor.DeclaringType;

            for (int i = 0; i < this.keys.Length; i++)
            {
                if (this.keys[i] == null)
                    break;

                if (this.keys[i].Equals(type, parameters))
                {
                    activator = this.activators[i];
                    break;
                }
            }

            if (activator != null)
                return activator(args);

            activator = GetActivator(ctor);
            this.Add(ctor.DeclaringType, parameters, activator);
            return activator(args);
        }

        public object CreateInstance(Type type, object[] args)
        {
            var types = args == null || args.Length == 0 ? Type.EmptyTypes : args.Select(x => x?.GetType() ?? null).ToArray();
            for (int i = 0; i < this.keys.Length; i++)
            {
                if (this.keys[i] == null)
                {
                    var ctors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                    for (int y = 0; y < ctors.Length; y++)
                        this.AddRange(ctors);
                }

                if (this.keys[i].Equals(type))
                {
                    if (this.keys[i].constructors == 1)
                        return this.activators[i](args);

                    for (int y = i; y < this.length; y++)
                    {
                        if (this.keys[y].parameterInfos.MatchesArgumentTypes(types))
                            return this.activators[y](args);
                    }
                }
            }

            throw new MissingMethodException($"A constructor with the given arguments was not found in type '{type.FullName}'");
        }

        public object CreateInstance(Type type)
        {
            ObjectActivator activator = null;

            for (int i = 0; i < this.keys.Length; i++)
            {
                if (this.keys[i] == null)
                    break;

                if (this.keys[i].Equals(type))
                    return this.activators[i]();
            }

            // Get the parameterless default constructor
            var ctor = type.GetConstructor(Type.EmptyTypes);

            if (ctor != null)
            {
                activator = GetActivator(ctor);
                this.Add(ctor.DeclaringType, null, activator);
                return activator();
            }

#if WINDOWS_UWP || NETCORE

            ctor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(x => x.GetParameters().Length == 0);
#else
            ctor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);

#endif
            if (ctor == null)
                throw new MissingMethodException($"Unable to find parameterless constructor in type {type.FullName}");

            activator = GetActivator(ctor);
            this.Add(ctor.DeclaringType, null, activator);
            return activator();
        }

        public object CreateInstance(ConstructorInfo ctor)
        {
            ObjectActivator activator = null;
            var type = ctor.DeclaringType;

            for (int i = 0; i < this.keys.Length; i++)
            {
                if (this.keys[i] == null)
                    break;

                if (this.keys[i].Equals(type))
                {
                    activator = this.activators[i];
                    break;
                }
            }

            if (activator != null)
                return activator();

            activator = GetActivator(ctor);
            this.Add(ctor.DeclaringType, null, activator);
            return activator();
        }

        private static ObjectActivator GetActivator(ConstructorInfo ctor)
        {
            // from https://rogeralsing.com/2008/02/28/linq-expressions-creating-objects/
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

        private void Add(Type type, ParameterInfo[] parameterInfos, ObjectActivator activator, int constructors = 1)
        {
            lock (syncObject)
            {
                if (this.HasType(type))
                    return;

                if (activators.Length == length)
                {
                    var newActivators = new ObjectActivator[activators.Length + 30];
                    var newKeys = new ActivatorArrayKey[keys.Length + 30];

                    Array.Copy(activators, newActivators, activators.Length);
                    Array.Copy(keys, newKeys, keys.Length);
                }

                activators[length] = activator;
                keys[length] = new ActivatorArrayKey { parameterInfos = parameterInfos, type = type, constructors = constructors };

                length++;
            }
        }

        private void AddRange(ConstructorInfo[] ctors)
        {
            lock (syncObject)
            {
                if (this.HasType(ctors[0].DeclaringType))
                    return;

                if (activators.Length <= length + ctors.Length)
                {
                    var newActivators = new ObjectActivator[activators.Length + 30 + ctors.Length];
                    var newKeys = new ActivatorArrayKey[keys.Length + 30 + ctors.Length];

                    Array.Copy(activators, newActivators, activators.Length);
                    Array.Copy(keys, newKeys, keys.Length);
                }

                for (int i = 0; i < ctors.Length; i++)
                {
                    var ctor = ctors[i];
                    activators[length] = GetActivator(ctor);
                    keys[length] = new ActivatorArrayKey { parameterInfos = ctor.GetParameters(), type = ctor.DeclaringType, constructors = ctors.Length };

                    length++;
                }
            }
        }

        private bool HasType(Type type)
        {
            for (int i = 0; i < this.length; i++)
            {
                if (this.keys[i].Equals(type))
                    return true;
            }

            return false;
        }
    }

    internal class ActivatorArrayKey
    {
        public int constructors;
        public ParameterInfo[] parameterInfos;
        public Type type;

        public bool Equals(Type type, ParameterInfo[] parameterInfos)
        {
            if (this.type != type)
                return false;

            if (parameterInfos.Length != this.parameterInfos.Length)
                return false;

            for (int i = 0; i < parameterInfos.Length; i++)
            {
                if (parameterInfos[i].ParameterType != this.parameterInfos[i].ParameterType)
                    return false;
            }

            return true;
        }

        public bool Equals(Type type) =>
            this.type.GetHashCode() == type.GetHashCode() && this.type.FullName.GetHashCode() == type.FullName.GetHashCode() && this.type.FullName == type.FullName;
    }
}