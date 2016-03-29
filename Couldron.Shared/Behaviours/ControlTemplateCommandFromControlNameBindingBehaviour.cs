using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Couldron.Behaviours
{
    public sealed class ControlTemplateCommandFromControlNameBindingBehaviour : Behaviour<Button>
    {
        #region Dependency Property ParentType

        /// <summary>
        /// Identifies the <see cref="ParentType" /> dependency property
        /// </summary>
        public static readonly DependencyProperty ParentTypeProperty = DependencyProperty.Register(nameof(ParentType), typeof(Type), typeof(ControlTemplateCommandFromControlNameBindingBehaviour), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="ParentType" /> Property
        /// </summary>
        public Type ParentType
        {
            get { return (Type)this.GetValue(ParentTypeProperty); }
            set { this.SetValue(ParentTypeProperty, value); }
        }

        #endregion Dependency Property ParentType

        protected override void OnAttach()
        {
        }

        protected override void OnDataContextChanged()
        {
            if (ParentType == null)
                return;

            var parent = this.AssociatedObject.FindParent(this.ParentType) as FrameworkElement;

            if (parent == null)
                return;

            this.AssociatedObject.SetBinding(Button.CommandProperty, this.AssociatedObject.DataContext, parent.Name + "Command");
        }

        protected override void OnDetach()
        {
        }
    }
}