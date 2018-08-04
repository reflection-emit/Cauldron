namespace Cauldron.Activator
{
    /// <summary>
    /// Represents a singleton access to an implementation of <typeparamref name="T"/>. <typeparamref
    /// name="T"/> is not neccessarily a singleton instance.
    /// </summary>
    /// <typeparam name="T">The type that is contained in the <see cref="Factory{T}"/></typeparam>
    public abstract class Factory<T> where T : class
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
                            current = Factory.Create<T>(callingType: typeof(T));
                    }
                }

                return current;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// <para/>
        /// This does not dispose the singleton object itself, only the content of <see
        /// cref="Current"/>. This will also destroy the singleton instance that is managed by the
        /// <see cref="Factory"/>.
        /// </summary>
        public void Free()
        {
            if (current != null)
            {
                lock (syncRoot)
                {
                    if (Factory.HasContract<T>())
                        Factory.Destroy<T>();

                    current?.TryDispose();
                    current = null;
                }
            }
        }
    }
}