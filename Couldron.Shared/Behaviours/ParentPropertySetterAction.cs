using System;

#if NETFX_CORE
using Windows.UI.Xaml;

#else

using System.Windows;

#endif

namespace Couldron.Behaviours
{
    /// <summary>
    /// Represents an action that is able to set a property of targeted control
    /// </summary>
    public sealed class ParentPropertySetterAction : ActionBase
    {
        #region Dependency Property TargetType

        /// <summary>
        /// Identifies the <see cref="TargetType" /> dependency property
        /// </summary>
        public static readonly DependencyProperty TargetTypeProperty = DependencyProperty.Register(nameof(TargetType), typeof(Type), typeof(ParentPropertySetterAction), new PropertyMetadata(null, ParentPropertySetterAction.OnTargetTypeChanged));

        /// <summary>
        /// Gets or sets the <see cref="TargetType" /> Property
        /// </summary>
        public Type TargetType
        {
            get { return (Type)this.GetValue(TargetTypeProperty); }
            set { this.SetValue(TargetTypeProperty, value); }
        }

        private static void OnTargetTypeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var d = dependencyObject as ParentPropertySetterAction;

            if (d == null)
                return;

            d.GetTheTargetControl();
        }

        #endregion Dependency Property TargetType

        #region Dependency Property PropertyValue

        /// <summary>
        /// Identifies the <see cref="PropertyValue" /> dependency property
        /// </summary>
        public static readonly DependencyProperty PropertyValueProperty = DependencyProperty.Register(nameof(PropertyValue), typeof(object), typeof(ParentPropertySetterAction), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="PropertyValue" /> Property
        /// </summary>
        public object PropertyValue
        {
            get { return (object)this.GetValue(PropertyValueProperty); }
            set { this.SetValue(PropertyValueProperty, value); }
        }

        #endregion Dependency Property PropertyValue

        #region Dependency Property PropertyName

        /// <summary>
        /// Identifies the <see cref="PropertyName" /> dependency property
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(nameof(PropertyName), typeof(string), typeof(ParentPropertySetterAction), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="PropertyName" /> Property
        /// </summary>
        public string PropertyName
        {
            get { return (string)this.GetValue(PropertyNameProperty); }
            set { this.SetValue(PropertyNameProperty, value); }
        }

        #endregion Dependency Property PropertyName

        /// <summary>
        /// Occures when the action is invoked by an event
        /// </summary>
        /// <param name="parameter">The parameter passed by the event</param>
        public override void Invoke(object parameter)
        {
            if (this.parent == null)
                return;

            var property = this.parent.GetType().GetProperty(this.PropertyName);

            if (property == null)
                throw new Exception("The property '" + this.PropertyName + "' could not be found in type '" + this.parent.GetType().FullName + "'");

            property.SetValue(this.parent, this.PropertyValue);
        }

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach()
        {
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
        }

        private object parent;

        private void GetTheTargetControl()
        {
            if (this.TargetType != null)
            {
                this.parent = this.AssociatedObject.FindVisualParent(this.TargetType);

#if !NETFX_CORE
                if (this.parent == null)
                    this.parent = this.AssociatedObject.FindLogicalParent(this.TargetType);
#endif
            }
        }
    }
}