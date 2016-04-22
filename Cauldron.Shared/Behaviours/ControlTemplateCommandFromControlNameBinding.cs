using System;
using System.Windows;
using System.Windows.Input;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else

using System.Windows.Controls;

#endif

namespace Cauldron.Behaviours
{
    /// <summary>
    /// Provides a behaviour that allows Buttons in <see cref="ControlTemplate"/> to bind to a Command that differs on every control.
    /// <para />
    /// Usecase example:
    /// <para/>
    /// Two <see cref="TextBox"/>ex with <see cref="ControlTemplate"/>s. Both <see cref="TextBox"/>es shares the <see cref="ControlTemplate"/>.
    /// A <see cref="Button"/> resides in the <see cref="ControlTemplate"/>. The <see cref="Button"/> on each <see cref="TextBox"/> must invoke
    /// two different <see cref="ICommand"/>s.
    /// </summary>
    public sealed class ControlTemplateCommandFromControlNameBinding : Behaviour<Button>
    {
        #region Dependency Property ParentType

        /// <summary>
        /// Gets or sets the <see cref="ParentType" /> Property
        /// </summary>
        public Type ParentType
        {
            get { return (Type)this.GetValue(ParentTypeProperty); }
            set { this.SetValue(ParentTypeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ParentType" /> dependency property
        /// </summary>
        public static readonly DependencyProperty ParentTypeProperty = DependencyProperty.Register(nameof(ParentType), typeof(Type), typeof(ControlTemplateCommandFromControlNameBinding), new PropertyMetadata(null, ControlTemplateCommandFromControlNameBinding.OnParentTypeChanged));

        private static void OnParentTypeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var d = dependencyObject as ControlTemplateCommandFromControlNameBinding;

            if (d == null)
                return;

            d.SetCommandBinding();
        }

        #endregion Dependency Property ParentType

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach()
        {
        }

        private void SetCommandBinding()
        {
            if (ParentType == null)
                return;

            var parent = this.AssociatedObject.FindVisualParent(this.ParentType) as FrameworkElement;

            if (parent == null)
                return;

            this.AssociatedObject.SetBinding(Button.CommandProperty, this.AssociatedObject.DataContext, parent.Name + "Command");
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
        }
    }
}