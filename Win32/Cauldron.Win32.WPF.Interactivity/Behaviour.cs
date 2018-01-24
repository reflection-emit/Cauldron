using Cauldron.Activator;
using System;
using System.ComponentModel;

#if WINDOWS_UWP

using Windows.UI.Xaml;

#else

using System.Windows;

#endif

namespace Cauldron.XAML.Interactivity
{
    /// <summary>
    /// A base class for behaviours
    /// </summary>
    /// <typeparam name="T">The control type the behaviour can be attached to</typeparam>
    public abstract class Behaviour<T> : FrameworkElement, IBehaviour where T : DependencyObject
    {
        private bool isdetached = false;

        /// <summary>
        /// Initializes a new instance of <see cref="Behaviour{T}"/>
        /// </summary>
        public Behaviour()
        {
        }

        /// <summary>
        /// Gets the associated object instance of <typeparamref name="T"/>
        /// </summary>
        public T AssociatedObject => (this as IBehaviour).AssociatedObject as T;

        object IBehaviour.AssociatedObject { get; set; }

        /// <summary>
        /// Gets a value that indicates if the behaviour was assigned from template
        /// </summary>
        public bool IsAssignedFromTemplate { get; private set; }

        /// <summary>
        /// Gets a value that indicates that the behaviour is detached
        /// </summary>
        public bool IsDetached { get { return this.isdetached; } }

        void IBehaviour.Attach()
        {
            this.OnAttach();
            this.isdetached = false;
        }

        IBehaviour IBehaviour.Copy()
        {
            var type = this.GetType();
            var behaviour = this.DeepClone();
            this.OnCopy(behaviour);

            behaviour.IsAssignedFromTemplate = true;
            return behaviour;
        }

        void IBehaviour.DataContextChanged(object newDataContext)
        {
            this.DataContext = newDataContext;
            this.OnDataContextChanged();
        }

        void IBehaviour.DataContextPropertyChanged(string name) => this.OnDataContextPropertyChanged(name);

        void IBehaviour.Detach()
        {
            if (this.isdetached)
                return;

            this.OnDetach();
            this.isdetached = true;
        }

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected abstract void OnAttach();

        /// <summary>
        /// Occures after shallow copying the behavior
        /// </summary>
        /// <param name="behaviour">The resulting behavior from <see cref="IBehaviour.Copy"/></param>
        protected virtual void OnCopy(IBehaviour behaviour)
        {
        }

        /// <summary>
        /// Occures if the <see cref="FrameworkElement.DataContext"/> of <see cref="Behaviour{T}.AssociatedObject"/> has changed
        /// </summary>
        protected virtual void OnDataContextChanged()
        {
        }

        /// <summary>
        /// Occures if the <see cref="FrameworkElement.DataContext"/> has invoked the <see cref="INotifyPropertyChanged.PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected virtual void OnDataContextPropertyChanged(string propertyName)
        {
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected abstract void OnDetach();
    }
}