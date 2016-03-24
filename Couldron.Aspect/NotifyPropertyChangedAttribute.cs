using Couldron.ViewModels;
using PostSharp.Aspects;
using System;
using System.ComponentModel;

namespace Couldron.Aspects
{
    /// <summary>
    /// Aspect that, when applied to a property intercepts the invocation of set and fires the RaiseNotifyPropertyChanged event.
    /// </summary>
    /// <exception cref="NotSupportedException">If instance does not inherits from <see cref="ViewModelBase"/></exception>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    [Serializable]
    public sealed class NotifyPropertyChangedAttribute : LocationInterceptionAspect
    {
        private string[] alsoNotifyChangeProperties;

        /// <summary>
        /// Initializes a new instance of <see cref="NotifyPropertyChangedAttribute"/>
        /// </summary>
        public NotifyPropertyChangedAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NotifyPropertyChangedAttribute"/>
        /// </summary>
        /// <param name="alsoNotifyChangeProperties">An array of Propertynames that also requires a <see cref="INotifyPropertyChanged.PropertyChanged"/> event invokation whenever this property is changed</param>
        public NotifyPropertyChangedAttribute(params string[] alsoNotifyChangeProperties)
        {
            this.alsoNotifyChangeProperties = alsoNotifyChangeProperties;
        }

        /// <summary>
        /// Method invoked instead of the Set semantic of the field or property to which the current aspect is applied, i.e. when the value of this field or property is changed.
        /// </summary>
        /// <param name="args">Advice arguments.</param>
        public override void OnSetValue(LocationInterceptionArgs args)
        {
            var type = args.Location.PropertyInfo.PropertyType;
            var oldValue = args.Location.GetValue(args.Instance);

            if (Utils.Equals(args.Location.PropertyInfo.PropertyType, args.Value, oldValue))
                return;

            base.OnSetValue(args);

            var vm = args.Instance as IViewModel;

            if (vm == null)
                throw new NotSupportedException("Adding the 'NotifyPropertyChangeAttribute' in a Type that does not implement 'IViewModel' is not supported");

            vm.RaiseNotifyPropertyChanged(args.LocationName);

            if (this.alsoNotifyChangeProperties != null && this.alsoNotifyChangeProperties.Length > 0)
            {
                for (int i = 0; i < this.alsoNotifyChangeProperties.Length; i++)
                    vm.RaiseNotifyPropertyChanged(this.alsoNotifyChangeProperties[i]);
            }
        }
    }
}