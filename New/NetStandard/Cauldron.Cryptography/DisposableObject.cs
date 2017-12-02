using System;
using System.Diagnostics.CodeAnalysis;

namespace Cauldron.Core
{
    /// <summary>
    /// Represents a base class implementation of <see cref="IDisposable"/>
    /// </summary>
    public abstract class DisposableObject : IDisposable
    {
        private volatile bool disposed = false;

        internal DisposableObject()
        {
        }

        /// <summary>
        /// Destructors are used to destruct instances of classes.
        /// </summary>
        ~DisposableObject()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        /// <param name="disposing">true if managed resources requires disposing</param>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        protected void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    this.OnDispose(true);
                }

                this.OnDispose(false);

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
        /// </summary>
        /// <param name="disposeManaged">true if managed resources requires disposing</param>
        protected abstract void OnDispose(bool disposeManaged);
    }
}