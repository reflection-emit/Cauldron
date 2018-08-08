using Cauldron;
using System;
using System.Linq;
using System.Reflection;
using Cauldron.Threading;
using Cauldron.Activator;

#if WINDOWS_UWP

using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

#else

using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;

#endif

namespace Cauldron.XAML.Interactivity.Actions
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

        #region Dependency Property MethodOwnerType

        /// <summary>
        /// Identifies the <see cref="MethodOwnerType" /> dependency property
        /// </summary>
        public static readonly DependencyProperty MethodOwnerTypeProperty = DependencyProperty.Register(nameof(MethodOwnerType), typeof(Type), typeof(InvokeMethodAction), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="MethodOwnerType" /> Property
        /// </summary>
        public Type MethodOwnerType
        {
            get { return (Type)this.GetValue(MethodOwnerTypeProperty); }
            set { this.SetValue(MethodOwnerTypeProperty, value); }
        }

        #endregion Dependency Property MethodOwnerType

        #region Dependency Property ParametersFromTemplatedParent

        /// <summary>
        /// Identifies the <see cref="ParametersFromTemplatedParent" /> dependency property
        /// </summary>
        public static readonly DependencyProperty ParametersFromTemplatedParentProperty = DependencyProperty.Register(nameof(ParametersFromTemplatedParent), typeof(bool), typeof(InvokeMethodAction), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the <see cref="ParametersFromTemplatedParent" /> Property
        /// </summary>
        public bool ParametersFromTemplatedParent
        {
            get { return (bool)this.GetValue(ParametersFromTemplatedParentProperty); }
            set { this.SetValue(ParametersFromTemplatedParentProperty, value); }
        }

        #endregion Dependency Property ParametersFromTemplatedParent

        #region Dependency Property PriorityLow

        /// <summary>
        /// Identifies the <see cref="PriorityLow" /> dependency property
        /// </summary>
        public static readonly DependencyProperty PriorityLowProperty = DependencyProperty.Register(nameof(PriorityLow), typeof(bool), typeof(InvokeMethodAction), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the <see cref="PriorityLow" /> Property
        /// </summary>
        public bool PriorityLow
        {
            get { return (bool)this.GetValue(PriorityLowProperty); }
            set { this.SetValue(PriorityLowProperty, value); }
        }

        #endregion Dependency Property PriorityLow

        [Inject]
        private new IDispatcher Dispatcher { get; set; }

        /// <summary>
        /// Occures when the action is invoked by an event
        /// </summary>
        /// <param name="parameter">The parameter passed by the event</param>
        public override void Invoke(object parameter)
        {
            if (string.IsNullOrEmpty(this.MethodName))
                throw new ArgumentNullException(nameof(this.MethodName), "Dependency Property 'MethodName' cannot be null");

            var element = this.AssociatedObject;
            var associatedObject = this.AssociatedObject;

            if (this.MethodOwnerType != null)
            {
                element = associatedObject.FindVisualParent(this.MethodOwnerType) as FrameworkElement;

#if !WINDOWS_UWP
                if (element == null)
                {
                    var contextMenu = associatedObject.FindVisualParent(typeof(ContextMenu)) as ContextMenu;
                    element = contextMenu.TemplatedParent.FindVisualParent(this.MethodOwnerType) as FrameworkElement;
                }
#endif
            }

            if (element == null)
                throw new NullReferenceException($"Unable to find the defined parent FrameworkElement of Type '{this.MethodOwnerType.FullName}'");

            if (!string.IsNullOrEmpty(this.MethodParameters))
            {
                var parameters = this.MethodParameters.Split(',');
                var method = element.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).FirstOrDefault(x => x.Name == this.MethodName && x.GetParameters().Length == parameters.Length);

                if (method == null)
                    throw new ArgumentException($"Unable to find a method with name '{this.MethodName}' in Type '{element.GetType().FullName}' that requires {parameters.Length} parameters");

                this.Invoke(method, element, this.GetParameters(parameters));
            }
            else
            {
                var method = element.GetType().GetMethod(this.MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

                if (method == null)
                    throw new ArgumentException($"Unable to find a method with name '{this.MethodName}' in Type '{element.GetType().FullName}'");

                this.Invoke(method, element, new object[] { });
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
#if WINDOWS_UWP
            var context = this.AssociatedObject;
            var property = context?.GetType().GetPropertyEx(propertyName);

            if (property == null)
                throw new ArgumentException($"Unable to find a property with name '{propertyName}' in Type '{(context == null ? this.AssociatedObject.GetType().FullName : context.GetType().FullName)}'");
#else
            var context = this.ParametersFromTemplatedParent ? this.AssociatedObject.TemplatedParent as FrameworkElement : this.AssociatedObject;
            var property = context?.GetType().GetPropertyEx(propertyName);

            if (property == null)
            {
                context = this.AssociatedObject.TemplatedParent as FrameworkElement;
                property = context?.GetType().GetPropertyEx(propertyName);

                if (property == null)
                    throw new ArgumentException($"Unable to find a property with name '{propertyName}' in Type '{(context == null ? this.AssociatedObject.GetType().FullName : context.GetType().FullName)}'");
            }

#endif
            return property.GetValue(context);
        }

        private async void Invoke(MethodInfo method, object obj, object[] parameters)
        {
            if (this.PriorityLow)
                await this.Dispatcher.RunAsync(DispatcherPriority.Low, () => method.Invoke(obj, parameters));
            else
                method.Invoke(obj, parameters);
        }
    }
}