using Cauldron.Core;
using Cauldron.Interception;
using System;
using System.ComponentModel;

namespace Cauldron.XAML.ViewModels
{
    /// <summary>
    /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event if the property is changed.
    /// <para />
    /// ATTENTION: Only use this if the PropertyChanged.Fody DoNotNotifyAttribute is applied on the property or
    /// this implementation will collide heavily with the implementation of PropertyChanged.Fody.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class RaisePropertyChangeAttribute : Attribute, IPropertySetterInterceptor
    {
        private bool canIsChanged;

        /// <summary>
        /// Initializes a new instance of <see cref="RaisePropertyChangeAttribute"/>.
        /// </summary>
        /// <param name="alsoTryChangeIsChangeProperty">If true tries to <see cref="IChangeAwareViewModel.IsChanged"/> property to true.</param>
        public RaisePropertyChangeAttribute(bool alsoTryChangeIsChangeProperty = false) => this.canIsChanged = alsoTryChangeIsChangeProperty;

        /// <exclude />
        public void OnException(Exception e)
        {
        }

        /// <exclude />
        public void OnExit()
        {
        }

        /// <exclude />
        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            if (Comparer.Equals(oldValue, newValue))
                return true;

            propertyInterceptionInfo.SetValue(newValue);
            (propertyInterceptionInfo.Instance as IViewModel)?.RaisePropertyChanged(propertyInterceptionInfo.PropertyName);

            if (this.canIsChanged && propertyInterceptionInfo.Instance is IChangeAwareViewModel changeAware)
            {
                changeAware.IsChanged = true;
                changeAware.RaisePropertyChanged(propertyInterceptionInfo.PropertyName, oldValue, newValue);
            }
            return true;
        }
    }
}