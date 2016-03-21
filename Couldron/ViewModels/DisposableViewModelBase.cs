using Couldron.Core;
using Couldron.Validation;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Couldron.ViewModels
{
    /// <summary>
    /// Represents the Base class of a ViewModel that implements the <see cref="IDisposable"/>
    /// </summary>
    public abstract class DisposableViewModelBase : ViewModelBase, IDisposableObject
    {
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of <see cref="DisposableViewModelBase"/>
        /// </summary>
        [InjectionConstructor]
        public DisposableViewModelBase() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DisposableViewModelBase"/>
        /// </summary>
        /// <param name="id">A unique identifier of the viewmodel</param>
        public DisposableViewModelBase(Guid id) : base(id)
        {
        }

        /// <summary>
        /// Destructors are used to destruct instances of classes.
        /// </summary>
        ~DisposableViewModelBase()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Occures if the object has been disposed
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Gets a value indicating if the object has been disposed or not
        /// </summary>
        [SuppressIsChanged]
        public bool IsDisposed { get { return this.disposed; } }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
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
                    DisposableUtils.DisposeObjects(this);
                }

                this.OnDispose(false);

                // Note disposing has been done.
                disposed = true;

                if (this.Disposed != null)
                    this.Disposed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
        /// </summary>
        /// <param name="disposeManaged">true if managed resources requires disposing</param>
        protected virtual void OnDispose(bool disposeManaged)
        {
        }
    }
}