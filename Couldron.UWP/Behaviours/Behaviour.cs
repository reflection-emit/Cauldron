using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Couldron.Behaviours
{
    public abstract partial class Behaviour<T>
    {
        #region Dependency Property DataContext

        /// <summary>
        /// Identifies the DataContext dependency property
        /// </summary>
        public static readonly DependencyProperty DataContextProperty = DependencyProperty.Register("DataContext", typeof(object), typeof(Behaviour<T>), new PropertyMetadata(null, Behaviour<T>.OnDataContextChanged));

        /// <summary>
        /// Gets or sets the DataContext Property
        /// </summary>
        public object DataContext
        {
            get { return (object)this.GetValue(DataContextProperty); }
            set { this.SetValue(DataContextProperty, value); }
        }

        private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var dependencyObject = d as Behaviour<T>;

            if (dependencyObject == null)
                return;

            dependencyObject.OnDataContextChanged();

            if (dependencyObject.DataContextChanged != null)
                dependencyObject.DataContextChanged(dependencyObject, args);
        }

        #endregion Dependency Property DataContext

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        private void Attach()
        {
            this.AssociatedObject.SetBinding(FrameworkElement.DataContextProperty, this, nameof(DataContext), BindingMode.TwoWay);

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
            this._associatedObject.Loaded -= TargetLoaded;
            this._associatedObject.Unloaded -= TargetUnloaded;

            this.OnDetach();
        }
    }
}