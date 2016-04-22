using System;
using System.Linq;
using System.Reflection;

#if NETFX_CORE
using Windows.UI.Xaml;
#else

using System.Windows;
using System.Windows.Controls;

#endif

namespace Cauldron.Behaviours
{
    /// <summary>
    /// Represents an action, that can invoke a method (also with parameters) residing on the the <see cref="Behaviour{T}.AssociatedObject"/> or in the control defined by <see cref="MethodOwnerType"/>.
    /// The method parameters must be properties of the <see cref="Behaviour{T}.AssociatedObject"/> or it's templatedparent (Not available in UWP)
    /// </summary>
    public sealed class InvokeMethodAction : ActionBase
    {
        #region Dependency Property MethodParameters

        /// <summary>
        /// Identifies the <see cref="MethodParameters" /> dependency property
        /// </summary>
        public static readonly DependencyProperty MethodParametersProperty = DependencyProperty.Register(nameof(MethodParameters), typeof(string), typeof(InvokeMethodAction), new PropertyMetadata(""));

        /// <summary>
        /// Gets or sets the <see cref="MethodParameters" /> Property
        /// </summary>
        public string MethodParameters
        {
            get { return (string)this.GetValue(MethodParametersProperty); }
            set { this.SetValue(MethodParametersProperty, value); }
        }

        #endregion Dependency Property MethodParameters

        #region Dependency Property MethodName

        /// <summary>
        /// Identifies the <see cref="MethodName" /> dependency property
        /// </summary>
        public static readonly DependencyProperty MethodNameProperty = DependencyProperty.Register(nameof(MethodName), typeof(string), typeof(InvokeMethodAction), new PropertyMetadata(""));

        /// <summary>
        /// Gets or sets the <see cref="MethodName" /> Property
        /// </summary>
        public string MethodName
        {
            get { return (string)this.GetValue(MethodNameProperty); }
            set { this.SetValue(MethodNameProperty, value); }
        }

        #endregion Dependency Property MethodName

        #region Dependency Property MyProperty

        /// <summary>
        /// Gets or sets the <see cref="MethodOwnerType" /> Property
        /// </summary>
        public Type MethodOwnerType
        {
            get { return (Type)this.GetValue(MethodOwnerTypeProperty); }
            set { this.SetValue(MethodOwnerTypeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MethodOwnerType" /> dependency property
        /// </summary>
        public static readonly DependencyProperty MethodOwnerTypeProperty = DependencyProperty.Register(nameof(MethodOwnerType), typeof(Type), typeof(InvokeMethodAction), new PropertyMetadata(null));

        #endregion Dependency Property MyProperty

        /// <summary>
        /// Occures when the action is invoked by an event
        /// </summary>
        /// <param name="parameter">The parameter passed by the event</param>
        public override void Invoke(object parameter)
        {
            if (string.IsNullOrEmpty(this.MethodName))
                throw new ArgumentNullException(nameof(this.MethodName), "Dependency Property 'MethodName' cannot be null");

            FrameworkElement element = this.AssociatedObject;

            if (this.MethodOwnerType != null)
            {
                element = this.AssociatedObject.FindVisualParent(this.MethodOwnerType) as FrameworkElement;

#if !NETFX_CORE
                if (element == null)
                {
                    var contextMenu = this.AssociatedObject.FindVisualParent(typeof(ContextMenu)) as ContextMenu;
                    element = contextMenu.TemplatedParent.FindVisualParent(this.MethodOwnerType) as FrameworkElement;
                }
#endif
            }

            if (element == null)
                throw new NullReferenceException("Unable to find the defined parent FrameworkElement of Type '" + this.MethodOwnerType.FullName + "'");

            if (!string.IsNullOrEmpty(this.MethodParameters))
            {
                var parameters = this.MethodParameters.Split(',');
                var method = element.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).FirstOrDefault(x => x.Name == this.MethodName && x.GetParameters().Length == parameters.Length);

                if (method == null)
                    throw new ArgumentException("Unable to find a method with name '" + this.MethodName + "' in Type '" + element.GetType().FullName + "' that requires " + parameters.Length + " parameters");

                method.Invoke(element, this.GetParameters(parameters));
            }
            else
            {
                var method = element.GetType().GetMethod(this.MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

                if (method == null)
                    throw new ArgumentException("Unable to find a method with name '" + this.MethodName + "' in Type '" + element.GetType().FullName + "'");

                method.Invoke(element, new object[] { });
            }
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

        private object[] GetParameters(string[] propertyNames)
        {
            var result = new object[propertyNames.Length];

            for (int i = 0; i < propertyNames.Length; i++)
                result[i] = this.GetPropertyValue(propertyNames[i].Trim());

            return result;
        }

        private object GetPropertyValue(string propertyName)
        {
            var context = this.AssociatedObject;
            var property = context.GetType().GetProperty(propertyName);

            if (property == null)
            {
#if !NETFX_CORE
                context = this.AssociatedObject.TemplatedParent as FrameworkElement;
                property = context?.GetType().GetProperty(propertyName);
#endif
                if (property == null)
                    throw new ArgumentException("Unable to find a property with name '" + propertyName + "' in Type '" + context == null ? this.AssociatedObject.GetType().FullName : context.GetType().FullName + "'");
            }

            return property.GetValue(context);
        }
    }
}