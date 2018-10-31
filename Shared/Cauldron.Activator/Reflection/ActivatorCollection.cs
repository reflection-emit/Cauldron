using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Cauldron.Reflection
{
    internal class ActivatorCollection
    {
        private ConcurrentDictionary<string, ActivatorKey[]> activators = new ConcurrentDictionary<string, ActivatorKey[]>();

        private object syncObject = new object();

        public object CreateInstance(ConstructorInfo ctor, object[] args)
        {
            var type = ctor.DeclaringType;

            if (this.activators.TryGetValue(type.FullName, out ActivatorKey[] keys))
                return ExpressionActivator.Activate(keys, args);

            return ExpressionActivator.Activate(this.Add(type), args);
        }

        public object CreateInstance(Type type, object[] args)
        {
            if (this.activators.TryGetValue(type.FullName, out ActivatorKey[] keys))
                return ExpressionActivator.Activate(keys, args);

            return ExpressionActivator.Activate(this.Add(type), args);
        }

        public object CreateInstance(Type type)
        {
            if (this.activators.TryGetValue(type.FullName, out ActivatorKey[] keys))
                return ExpressionActivator.Activate(keys, null);

            return ExpressionActivator.Activate(this.Add(type), null);
        }

        public object CreateInstance(ConstructorInfo ctor)
        {
            var type = ctor.DeclaringType;

            if (this.activators.TryGetValue(type.FullName, out ActivatorKey[] keys))
                return ExpressionActivator.Activate(keys, null);

            return ExpressionActivator.Activate(this.Add(type), null);
        }

        private ActivatorKey[] Add(Type type)
        {
            if (this.activators.ContainsKey(type.FullName))
                return this.activators[type.FullName];

            lock (this.syncObject)
            {
                if (this.activators.ContainsKey(type.FullName))
                    return this.activators[type.FullName];

                var ctors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).OrderBy(x => x.GetParameters().Length).ToArray();
                var activatorKeys = new ActivatorKey[ctors.Length];

                for (int i = 0; i < activatorKeys.Length; i++)
                    activatorKeys[i] = new ActivatorKey { parameterInfos = ctors[i].GetParameters(), activator = ExpressionActivator.GetActivator(ctors[i]) };

                this.activators.TryAdd(type.FullName, activatorKeys);
                return activatorKeys;
            }
        }
    }
}