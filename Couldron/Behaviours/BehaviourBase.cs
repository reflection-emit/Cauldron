using Couldron.Core;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows;

namespace Couldron.Behaviours
{
    /// <summary>
    /// Represents a base for behaviours
    /// </summary>
    public abstract class BehaviourBase : DependencyObject, IDisposable
    {
        private bool disposed = false;

        /// <summary>
        /// Destructors are used to destruct instances of classes.
        /// </summary>
        ~BehaviourBase()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Occures if the object has been disposed
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Gets a value that indicates the behaviour was assigned from a template
        /// </summary>
        public bool IsAssignedFromTemplate { get; internal set; }

        /// <summary>
        /// Gets a value indicating if the object has been disposed or not
        /// </summary>
        public bool IsDisposed { get { return this.disposed; } }

        /// <summary>
        /// Creates a shallow copy of the instance
        /// </summary>
        /// <returns>A copy of the behaviour</returns>
        public BehaviourBase Copy()
        {
            var type = this.GetType();
            var behaviour = Activator.CreateInstance(type) as BehaviourBase;

            var props = type.GetProperties(System.Reflection.BindingFlags.Public).ToArray<PropertyInfo>();

            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];

                try
                {
                    // exclude ResourceDictionaries and Styles
                    if (prop.CanWrite && prop.CanRead && prop.PropertyType != typeof(ResourceDictionary) && prop.PropertyType != typeof(Style))
                        prop.SetValue(behaviour, prop.GetValue(this));
                }
                catch
                {
                    // Happens sometimes, but it's not important if something bad happens
                }
            }

            this.OnCopy(behaviour);

            behaviour.IsAssignedFromTemplate = true;
            return behaviour;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Sets the behaviour's associated object
        /// </summary>
        /// <param name="obj">The associated object</param>
        internal abstract void SetAssociatedObject(object obj);

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
                    this.OnDetach();
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
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected abstract void OnAttach();

        /// <summary>
        /// Occures after shallow copying the behavior
        /// </summary>
        /// <param name="behaviour">The resulting behavior from <see cref="BehaviourBase.Copy"/></param>
        protected virtual void OnCopy(BehaviourBase behaviour)
        {
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected abstract void OnDetach();

        /// <summary>
        /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
        /// </summary>
        /// <param name="disposeManaged">true if managed resources requires disposing</param>
        protected virtual void OnDispose(bool disposeManaged)
        {
        }
    }
}