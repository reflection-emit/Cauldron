using Cauldron.XAML.Interactivity.Actions;
using System;
using System.Windows;
using System.Windows.Markup;

namespace Cauldron.XAML.Interactivity
{
    /// <summary>
    /// Provides a <see cref="Behaviour{T}"/> that invokes actions when the bound data meets a specified condition.
    /// </summary>
    [ContentProperty("Actions")]
    public sealed class DataTrigger : Behaviour<FrameworkElement>
    {
        #region Dependency Property Binding

        /// <summary>
        /// Identifies the Binding dependency property
        /// </summary>
        public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding", typeof(object), typeof(DataTrigger), new PropertyMetadata(null, DataTrigger.OnBindingChanged));

        /// <summary>
        /// Gets or sets the Binding Property
        /// </summary>
        public object Binding
        {
            get => this.GetValue(BindingProperty);
            set => this.SetValue(BindingProperty, value);
        }

        private static void OnBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is DataTrigger dataTrigger)
                dataTrigger.InvokeActions();
        }

        #endregion Dependency Property Binding

        #region Dependency Property Value

        /// <summary>
        /// Identifies the <see cref="Value" /> dependency property
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(DataTrigger), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Value" /> Property
        /// </summary>
        public object Value
        {
            get => this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        #endregion Dependency Property Value

        #region Dependency Property IsInverted

        /// <summary>
        /// Identifies the <see cref="IsInverted" /> dependency property
        /// </summary>
        public static readonly DependencyProperty IsInvertedProperty = DependencyProperty.Register(nameof(IsInverted), typeof(bool), typeof(DataTrigger), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the <see cref="IsInverted" /> Property
        /// </summary>
        public bool IsInverted
        {
            get => (bool)this.GetValue(IsInvertedProperty);
            set => this.SetValue(IsInvertedProperty, value);
        }

        #endregion Dependency Property IsInverted

        private ActionCollection actions;

        /// <summary>
        /// Gets a collection of actions that can be invoked by this behaviour
        /// </summary>
        public ActionCollection Actions
        {
            get
            {
                if (this.actions == null)
                    this.actions = new ActionCollection(this.AssociatedObject);

                return this.actions;
            }
        }

        /// <exclude />
        protected override void OnAttach()
        {
        }

        /// <exclude />
        protected override void OnDetach()
        {
        }

        private void InvokeAction(object parameter)
        {
            foreach (var item in this.Actions)
                item.Invoke(parameter);
        }

        private void InvokeActions()
        {
            if (this.Binding == null && this.Value == null)
            {
                this.InvokeAction(null);
                return;
            }

            if (this.Binding == null)
                return;

            var value = this.Value == null ? null : Convert.ChangeType(this.Value, Convert.GetTypeCode(this.Binding));
            var actionParameter =
                (!this.IsInverted && Comparer.Equals(this.Binding, value)) ||
                (this.IsInverted && Comparer.UnEquals(this.Binding, value));

            this.InvokeAction(actionParameter);
        }
    }
}