using Cauldron.Core;
using Cauldron.Core.Extensions;

namespace Cauldron.Activator
{
    /// <summary>
    /// Represents a testible singleton implementation of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type that is contained in the singleton</typeparam>
    public abstract class Singleton<T> where T : class
    {
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
                            current = Factory.Create<T>();
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
                    current?.TryDispose();
                    current = null;
                }
            }
        }
    }
}