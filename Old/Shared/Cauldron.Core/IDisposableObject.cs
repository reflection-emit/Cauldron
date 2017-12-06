using System;

namespace Cauldron.Core
{
    /// <summary>
    /// Provides a mechanism for releasing unmanaged resources.
    /// </summary>
    public interface IDisposableObject : IDisposable
    {
        /// <summary>
        /// Occures if the object has been disposed
        /// </summary>
        event EventHandler Disposed;

        /// <summary>
        /// Gets a value indicating if the object has been disposed or not
        /// </summary>
        bool IsDisposed { get; }
    }
}