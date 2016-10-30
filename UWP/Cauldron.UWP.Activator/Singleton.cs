using Cauldron.Core;
using Cauldron.Core.Extensions;
using System;
using System.Reflection;

namespace Cauldron.Activator
{
    /// <summary>
    /// Represents a singleton implementation of <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type that is contained in the singleton</typeparam>
    public abstract class Singleton<T> where T : class
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
                var disposable = current as IDisposableObject;

                if (current == null || (disposable != null && disposable.IsDisposed))
                {
                    lock (syncRoot)
                    {
                        if (current == null || (disposable != null && disposable.IsDisposed))
                        {
                            var type = typeof(T);
#if WINDOWS_UWP
                            var attr = type.GetTypeInfo().GetCustomAttribute<ComponentAttribute>();
#else
                            var attr = type.GetCustomAttribute<ComponentAttribute>();
#endif
                            if (attr == null)
                                current = Factory.Create<T>();
                            else
                            {
                                contractName = attr.ContractName;
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
        public void Free()
        {
            if (current != null)
            {
                lock (syncRoot)
                {
                    var disposable = current.As<IDisposable>();

                    if (disposable != null)
                    {
                        if (string.IsNullOrEmpty(contractName))
                            disposable.Dispose();
                        else
                            Factory.Destroy(contractName);
                    };
                    current = null;
                    contractName = null;
                }
            }
        }
    }
}