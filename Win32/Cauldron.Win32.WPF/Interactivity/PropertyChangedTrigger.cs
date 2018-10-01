using Cauldron.XAML.Interactivity.Actions;

#if WINDOWS_UWP

using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

#else

using System.Windows.Markup;
using System.Windows.Data;
using System.Windows;

#endif

namespace Cauldron.XAML.Interactivity
{
    /// <summary>
    /// Provides a <see cref="Behaviour{T}"/> that can invoke <see cref="ActionBase"/> behaviours if a property changes
    /// </summary>
#if WINDOWS_UWP

    [ContentProperty(Name = nameof(Actions))]
#else

    [ContentProperty("Actions")]
#endif
    public sealed class PropertyChangedTrigger : Behaviour<FrameworkElement>
    {
        private ActionCollection actions;

        #region Dependency Property Property

        /// <summary>
        /// Identifies the <see cref="Property" /> dependency property
        /// </summary>
        public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register(nameof(Property), typeof(DependencyProperty), typeof(PropertyChangedTrigger), new PropertyMetadata(null, PropertyChangedTrigger.OnPropertyChanged));

        /// <summary>
        /// Gets or sets the <see cref="Property" /> Property
        /// </summary>
        public DependencyProperty Property
        {
            get => this.GetValue(PropertyProperty) as DependencyProperty;
            set => this.SetValue(PropertyProperty, value);
        }

        private static void OnPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (dependencyObject is PropertyChangedTrigger propertyChangedTrigger)
                propertyChangedTrigger.GetBindedProperty();
        }

        #endregion Dependency Property Property

        private string bindedProperty;

        /// <summary>
        /// Gets a collection of actions that can be invoked by this behaviour
        /// </summary>
        public ActionCollection Actions
        {
            get
            {
                if (null == this.actions)
                    this.actions = new ActionCollection(this.AssociatedObject);

                return this.actions;
            }
        }

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach()
        {
        }

        /// <summary>
        /// Occures after shallow copying the behavior
        /// </summary>
        /// <param name="behaviour">The resulting behavior from <see cref="IBehaviour.Copy"/></param>
        protected override void OnCopy(IBehaviour behaviour)
        {
            var trigger = behaviour as PropertyChangedTrigger;

            foreach (var item in this.Actions)
                trigger.Actions.Add((item as IBehaviour).Copy() as ActionBase);
        }

        /// <summary>
        /// Occures if the <see cref="FrameworkElement.DataContext"/> of <see cref="Behaviour{T}.AssociatedObject"/> has changed
        /// </summary>
        protected override void OnDataContextChanged()
        {
            base.OnDataContextChanged();
            this.GetBindedProperty();
        }

        /// <summary>
        /// Occures if the <see cref="FrameworkElement.DataContext"/> has invoked the INotifyPropertyChanged.PropertyChanged event
        /// </summary>
        /// <param name="propertyName"></param>
        protected override void OnDataContextPropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(this.bindedProperty) || propertyName != this.bindedProperty)
                return;

            foreach (var item in this.Actions)
                item.Invoke(null);
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
        }

        private void GetBindedProperty()
        {
            if (this.Property == null || this.AssociatedObject == null)
                return;
#if WINDOWS_UWP
            var bindingExpression = this.AssociatedObject.GetBindingExpression(this.Property);
            var binding = bindingExpression.ParentBinding;
#else
            var binding = BindingOperations.GetBinding(this.AssociatedObject, this.Property);
#endif
            if (binding == null)
                return;

            var bindingPath = binding.Path.Path.Split('.');
            this.bindedProperty = bindingPath[bindingPath.Length - 1];
        }
    }
}