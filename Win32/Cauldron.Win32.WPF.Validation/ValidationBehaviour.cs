using Cauldron.XAML.Interactivity;
using Cauldron.XAML.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

#if WINDOWS_UWP

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

#else

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

#endif

namespace Cauldron.XAML.Validation
{
    /// <summary>
    /// Provides supporting functionalities for the validation
    /// </summary>
    public sealed class ValidationBehaviour : Behaviour<FrameworkElement>, ICloneable
    {
        private static ConcurrentDictionary<Type, IEnumerable<DependencyPropertyInfo>> dependencyPropertyCache = new ConcurrentDictionary<Type, IEnumerable<DependencyPropertyInfo>>();
        private Dictionary<string, string> allErrors = new Dictionary<string, string>();
        private Dictionary<string, WeakReference<INotifyDataErrorInfo>> validableProperties = new Dictionary<string, WeakReference<INotifyDataErrorInfo>>();

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone() => new ValidationBehaviour();

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach() => this.OnDataContextChanged();

        /// <summary>
        /// Occures if the <see cref="FrameworkElement.DataContext"/> of <see
        /// cref="Behaviour{T}.AssociatedObject"/> has changed
        /// </summary>
        protected override void OnDataContextChanged()
        {
            foreach (var item in this.validableProperties.Values)
                item.GetTarget().IsNotNull(x => x.ErrorsChanged -= this.Source_ErrorsChanged);

            this.validableProperties.Clear();

            var type = this.AssociatedObject.GetType();
            IEnumerable<DependencyPropertyInfo> dependencyProperties = null;

            if (dependencyPropertyCache.ContainsKey(type))
                dependencyProperties = dependencyPropertyCache[type];
            else
            {
                dependencyProperties = this.AssociatedObject.GetDependencyProperties();
                dependencyPropertyCache.TryAdd(type, dependencyProperties);
            }

            // Get all known Dependency Property with bindings
            foreach (var item in dependencyProperties)
            {
                if (item.Name == "IsEnabled" || item.Name == "Visibility")
                    continue;

                var bindingExpression = this.AssociatedObject.GetBindingExpression(item.DependencyProperty);
                this.SetValidationInfo(bindingExpression);
            }

            // if the associated element is a password box we have to do some exceptional stuff
            if (this.AssociatedObject is PasswordBox)
            {
                var passwordBinding = Interaction.GetBehaviour<PasswordBoxBinding>(this.AssociatedObject).FirstOrDefault();

                if (passwordBinding != null)
                {
                    var bindingExpression = passwordBinding.GetBindingExpression(PasswordBoxBinding.PasswordProperty);
                    this.SetValidationInfo(bindingExpression);
                }
            }
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
            this.allErrors.Clear();

            foreach (var item in this.validableProperties.Values)
                item.GetTarget().IsNotNull(x =>
                {
                    x.ErrorsChanged -= this.Source_ErrorsChanged;
                    (x as IValidatableViewModel).IsNotNull(y => y.Validating -= Source_Validating);
                });

            this.validableProperties.Clear();
        }

        private void SetValidationInfo(BindingExpression bindingExpression)
        {
            if (bindingExpression == null)
                return;

            // get the binding sources in order to do some voodoo on them
            var source = bindingExpression.ParentBinding.Source as INotifyDataErrorInfo;
            if (source == null)
                source = this.AssociatedObject.DataContext as INotifyDataErrorInfo;

            // we don't support too complicated binding paths
            var propertyName = bindingExpression.ParentBinding.Path.Path;

            // check if the property name end with a ']' ... We dont support that
            if (propertyName.Right(1) == "]")
                return;

            if (propertyName.Contains('.'))
            {
                source = XAMLHelper.GetSourceFromPath(source, propertyName) as INotifyDataErrorInfo;
                propertyName = propertyName.Right(propertyName.Length - propertyName.LastIndexOf('.') - 1);
            }

            // if our source is still null then either the source does not implements the
            // INotifyDataErrorInfo interface or we dont have a valid source at all In both cases we
            // just give up
            if (source == null)
                return;

            // then let us check if the binding source have validation attributes on them
            var property = source.GetType().GetPropertyEx(propertyName);

            if (property == null)
                return;

            if (!this.validableProperties.ContainsKey(propertyName) && property.GetCustomAttributes().Any(x => x is ValidatorAttributeBase))
            {
                this.validableProperties.Add(propertyName, new WeakReference<INotifyDataErrorInfo>(source));
                source.ErrorsChanged += this.Source_ErrorsChanged;
                (source as IValidatableViewModel).IsNotNull(x => x.Validating += Source_Validating);

                // Also... If we have a mandatory attribute... Add it
                if (property.GetCustomAttribute<IsMandatoryAttribute>() != null)
                    ValidationProperties.SetIsMandatory(this.AssociatedObject, true);
            }
        }

        private void Source_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            if (this.validableProperties.ContainsKey(e.PropertyName))
            {
                var context = this.validableProperties[e.PropertyName];
                var source = context.GetTarget();

                if (source == null)
                {
                    this.validableProperties.Remove(e.PropertyName);
                    return;
                }

                var errors = source.GetErrors(e.PropertyName) as IEnumerable<string>;
                this.allErrors.Remove(e.PropertyName);

                if (errors != null)
                    this.allErrors.Add(e.PropertyName, errors.Join("\r\n"));

                ValidationProperties.SetHasErrors(this.AssociatedObject, this.allErrors.Count > 0);
                ValidationProperties.SetErrors(this.AssociatedObject, this.allErrors.Count == 0 ? string.Empty : this.allErrors.Values.Join("\r\n\r\n"));
            }
        }

        private void Source_Validating(object sender, ValidationEventArgs e)
        {
            if (!this.validableProperties.ContainsKey(e.PropertyName))
                return;

            var context = this.validableProperties[e.PropertyName];
            var source = context.GetTarget();

            if (source == null)
            {
                this.validableProperties.Remove(e.PropertyName);
                return;
            }

            var property = source.GetType().GetPropertyEx(e.PropertyName);

            if (property == null)
                return;

            var attrib = property.GetCustomAttribute<IsMandatoryAttribute>();

            if (attrib == null || !attrib.IsDeactivatable)
                return;

            ValidationProperties.SetIsMandatory(this.AssociatedObject, attrib.IsEnabled(source, property));
        }
    }
}