using System;
using System.Windows;

namespace Couldron.Behaviours
{
    /// <summary>
    /// A base class for behaviours
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Behaviour<T> : BehaviourBase where T : FrameworkElement
    {
        private T _associatedObject;

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
                    this.OnDetach();

                this._associatedObject = value;

                if (this._associatedObject == null)
                    return;

                this.OnAttach();
            }
        }

        /// <summary>
        /// Sets the behaviour's associated object
        /// </summary>
        /// <param name="obj">The associated object</param>
        internal override void SetAssociatedObject(object obj)
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
        protected override void OnAttach()
        {
            this._associatedObject.DataContextChanged += AssociatedObjectDataContextChanged;
            this._associatedObject.Loaded += TargetLoaded;
            this._associatedObject.Unloaded += TargetUnloaded;

            // Initial invoke... If the DataContext is already set before anything
            if (this._associatedObject.DataContext != null)
                this.OnDataContextChanged();
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
        protected override void OnDetach()
        {
            this._associatedObject.DataContextChanged -= AssociatedObjectDataContextChanged;
            this._associatedObject.Loaded -= TargetLoaded;
            this._associatedObject.Unloaded -= TargetUnloaded;
        }

        /// <summary>
        /// Occures when the <see cref="Behaviour{T}.AssociatedObject"/> is loaded
        /// </summary>
        protected virtual void OnLoaded()
        {
        }

        private static void OnDataContextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            (dependencyObject as Behaviour<T>).IsNotNull(x => x.OnDataContextChanged());
        }

        private void AssociatedObjectDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.OnDataContextChanged();
        }

        private void TargetLoaded(object sender, RoutedEventArgs e)
        {
            this.OnLoaded();
        }

        private void TargetUnloaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsDisposed)
                this.OnDetach();
        }
    }
}