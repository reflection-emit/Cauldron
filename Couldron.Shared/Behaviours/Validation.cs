using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using Couldron.Validation;
using Couldron.Attached;
using System.Collections;

#if NETFX_CORE
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#else

using System.Windows.Controls;
using System.Windows;

#endif

namespace Couldron.Behaviours
{
    /// <summary>
    /// Provides supporting functionalities for the validation
    /// </summary>
    public sealed class Validation : Behaviour<FrameworkElement>
    {
        private Dictionary<string, WeakReference<INotifyDataErrorInfo>> validableProperties = new Dictionary<string, WeakReference<INotifyDataErrorInfo>>();

        /// <summary>
        /// Occures if the <see cref="FrameworkElement.DataContext"/> of <see cref="Behaviour{T}.AssociatedObject"/> has changed
        /// </summary>
        protected override void OnDataContextChanged()
        {
            foreach (var item in this.validableProperties.Values)
                item.GetTarget().IsNotNull(x => x.ErrorsChanged -= this.Source_ErrorsChanged);

            this.validableProperties.Clear();

            // Get all known Dependency Property with bindings
            foreach (var item in this.AssociatedObject.GetType().GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType == typeof(DependencyProperty)))
            {
                var bindingExpression = this.AssociatedObject.GetBindingExpression(item.GetValue(null) as DependencyProperty);

                if (bindingExpression == null)
                    continue;

                // get the binding sources in order to do some voodoo on them
                var source = bindingExpression.ParentBinding.Source as INotifyDataErrorInfo;
                if (source == null)
                    source = this.AssociatedObject.DataContext as INotifyDataErrorInfo;

                // we don't support too complicated binding paths
                var propertyName = bindingExpression.ParentBinding.Path.Path;

                // check if the property name end with a ']' ... We dont support that
                if (propertyName.Right(1) == "]")
                    continue;

                if (propertyName.Contains('.'))
                {
                    source = this.GetSource(source, propertyName) as INotifyDataErrorInfo;
                    propertyName = propertyName.Right(propertyName.Length - propertyName.LastIndexOf('.') - 1);
                }

                // if our source is still null then either the source does not implements the INotifyDataErrorInfo interface or we dont have a valid source at all
                // In both cases we just give up
                if (source == null)
                    continue;

                // then let us check if the binding source have validation attributes on them
                var property = source.GetType().GetProperty(propertyName);

                if (property == null)
                    continue;

                if (!this.validableProperties.ContainsKey(propertyName) && property.GetCustomAttributes().Any(x => x is ValidationBaseAttribute))
                {
                    this.validableProperties.Add(propertyName, new WeakReference<INotifyDataErrorInfo>(source));
                    source.ErrorsChanged += this.Source_ErrorsChanged;

                    // Also... If we have a mandatory attribute... Add it
                    if (property.GetCustomAttribute<IsMandatoryAttribute>() != null)
                        ValidationProperties.SetIsMandatory(this.AssociatedObject, true);
                }
            }
        }

        private object GetSource(object source, string path)
        {
            var bindingPath = path.Split('.');

            // let us follow the path and change the source accordingly
            for (int i = 0; i < bindingPath.Length - 1; i++)
            {
                if (source == null)
                    break;

                var section = bindingPath[i];

                // is this an array?
                if (section.Right(1) == "]")
                {
                    // lets get the indexer between []
                    var indexer = section.EnclosedIn('[', ']').ToInteger();
                    var name = section.Left(section.IndexOf('['));
                    var propertyInfo = source.GetType().GetProperty(name);

                    if (propertyInfo == null)
                    {
                        // the path is invalid...
                        source = null;
                        break;
                    }

                    var array = propertyInfo.GetValue(source) as IEnumerable;
                    source = array.ElementAt(indexer);
                }
                else
                {
                    var propertyInfo = source.GetType().GetProperty(section);

                    if (propertyInfo == null)
                    {
                        // the path is invalid...
                        source = null;
                        break;
                    }

                    source = propertyInfo.GetValue(source);
                }
            }

            return source;
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

                var errors = source.GetErrors(e.PropertyName) as List<string>;

                ValidationProperties.SetHasErrors(this.AssociatedObject, errors != null);
                ValidationProperties.SetErrors(this.AssociatedObject, errors == null ? string.Empty : string.Join("\r\n", errors));
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
            foreach (var item in this.validableProperties.Values)
                item.GetTarget().IsNotNull(x => x.ErrorsChanged -= this.Source_ErrorsChanged);

            this.validableProperties.Clear();
        }
    }
}