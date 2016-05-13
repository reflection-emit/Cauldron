using System;
using System.Reflection;

namespace Cauldron.Core
{
    /// <summary>
    /// Represents a singleton implementation of <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type that is contained in the singleton</typeparam>
    public abstract class Singleton<T> : IDisposable where T : Singleton<T>
    {
        private static string contractName = null;
        private static volatile T current;
        private static object syncRoot = new object();

        /// <summary>
        /// Gets the current instance of <typeparamref name="T"/>
        /// </summary>
        public static T Current
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                        {
                            var type = typeof(T);
                            var attr = type.GetTypeInfo().GetCustomAttribute<FactoryAttribute>();

                            if (attr == null)
                            {
                                contractName = null;
                                current = Factory.Create<T>();
                            }
                            else
                            {
                                contractName = typeof(T).FullName;
                                current = Factory.Create(contractName) as T;
                            }
                        }
                    }
                }

                return current;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// <para/>
        /// This does not dispose the singleton object itself, only the content of <see cref="Current"/>.
        /// </summary>
        public void Dispose()
        {
            if (current != null)
            {
                lock (syncRoot)
                {
                    current.CastTo<IDisposable>().IsNotNull(x =>
                    {
                        if (string.IsNullOrEmpty(contractName))
                            x.Dispose();
                        else
                            Factory.Destroy(contractName);
                    });
                    current = null;
                    contractName = null;
                }
            }
        }
    }
}