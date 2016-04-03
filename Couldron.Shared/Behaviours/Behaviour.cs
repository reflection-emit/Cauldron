using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

#else

using System.Windows;
using System.Windows.Data;

#endif

namespace Couldron.Behaviours
{
    /// <summary>
    /// A base class for behaviours
    /// </summary>
    /// <typeparam name="T">The control type the behaviour can be attached to</typeparam>
    public abstract partial class Behaviour<T> : FrameworkElement, IBehaviour<T> where T : FrameworkElement
    {
        private T _associatedObject;

        #region Dependency Property Name

        /// <summary>
        /// Identifies the <see cref="Name" /> dependency property
        /// </summary>
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register(nameof(Name), typeof(string), typeof(Behaviour<T>), new PropertyMetadata(""));

        /// <summary>
        /// Gets or sets the <see cref="Name" /> Property
        /// </summary>
        public string Name
        {
            get { return (string)this.GetValue(NameProperty); }
            set { this.SetValue(NameProperty, value); }
        }

        #endregion Dependency Property Name

        /// <summary>
        /// Gets the <see cref="DependencyObject"/> to which the behavior is attached.
        /// </summary>
        public T AssociatedObject
        {
            get { return this._associatedObject; }
            set
            {
                if (this._associatedObject == value)
                    return;

                if (this._associatedObject != null)
                    this.Detach();

                this._associatedObject = value;

                if (this._associatedObject == null)
                    return;

                this.Attach();
            }
        }

        /// <summary>
        /// Gets a value that indicates the behaviour was assigned from a template
        /// </summary>
        public bool IsAssignedFromTemplate { get; private set; }

        /// <summary>
        /// Creates a shallow copy of the instance
        /// </summary>
        /// <returns>A copy of the behaviour</returns>
        IBehaviour IBehaviour.Copy()
        {
            var type = this.GetType();
            var behaviour = Activator.CreateInstance(type) as Behaviour<T>;

            var props = type.GetProperties().ToArray<PropertyInfo>();

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
        /// Sets the behaviour's associated object
        /// </summary>
        /// <param name="obj">The associated object</param>
        void IBehaviour.SetAssociatedObject(object obj)
        {
            if (obj == null)
                return;

            this.AssociatedObject = obj as T;

            if (this._associatedObject == null)
                throw new Exception(string.Format("The Type of AssociatedObject \"{0}\" does not match with T \"{1}\"", obj.GetType(), typeof(T)));
        }

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected abstract void OnAttach();

        /// <summary>
        /// Occures after shallow copying the behavior
        /// </summary>
        /// <param name="behaviour">The resulting behavior from <see cref="IBehaviour.Copy"/></param>
        protected virtual void OnCopy(IBehaviour<T> behaviour)
        {
        }

        /// <summary>
        /// Occures if the <see cref="FrameworkElement.DataContext"/> of <see cref="Behaviour{T}.AssociatedObject"/> has changed
        /// </summary>
        protected virtual void OnDataContextChanged()
        {
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected abstract void OnDetach();

        /// <summary>
        /// Occures when the <see cref="Behaviour{T}.AssociatedObject"/> is loaded
        /// </summary>
        protected virtual void OnLoaded()
        {
        }

        private void TargetLoaded(object sender, RoutedEventArgs e)
        {
            this.OnLoaded();
        }

        private void TargetUnloaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsDisposed)
                this.Detach();
        }

        #region IDisposable

        private bool disposed = false;

        /// <summary>
        /// Destructors are used to destruct instances of classes.
        /// </summary>
        ~Behaviour()
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
                    this.Detach();
                    this.OnDispose(true);
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

        #endregion IDisposable
    }
}