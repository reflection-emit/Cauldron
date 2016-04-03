using System.Windows;
using System.Windows.Data;

namespace Couldron.Behaviours
{
    public abstract partial class Behaviour<T>
    {
        private void AssociatedObjectDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.OnDataContextChanged();
        }

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        private void Attach()
        {
            this.SetBinding(Behaviour<T>.DataContextProperty, this.AssociatedObject, nameof(FrameworkElement.DataContext), BindingMode.OneWay);

            this._associatedObject.DataContextChanged += AssociatedObjectDataContextChanged;
            this._associatedObject.Loaded += TargetLoaded;
            this._associatedObject.Unloaded += TargetUnloaded;

            // Initial invoke... If the DataContext is already set before anything
            if (this._associatedObject.DataContext != null)
                this.OnDataContextChanged();

            this.OnAttach();
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        private void Detach()
        {
            BindingOperations.ClearAllBindings(this);

            this._associatedObject.DataContextChanged -= AssociatedObjectDataContextChanged;
            this._associatedObject.Loaded -= TargetLoaded;
            this._associatedObject.Unloaded -= TargetUnloaded;

            this.OnDetach();
        }
    }
}