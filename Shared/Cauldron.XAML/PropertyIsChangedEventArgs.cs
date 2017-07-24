using Cauldron.XAML.ViewModels;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Cauldron.XAML
{
    /// <summary>
    /// Represents the arguments of <see cref="IChangeAwareViewModel.Changed"/>.
    /// </summary>
    public sealed class PropertyIsChangedEventArgs : EventArgs
    {
        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public PropertyIsChangedEventArgs(string propertyName, object oldValue, object newValue)
        {
            this.PropertyName = propertyName;
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        /// <summary>
        /// Gets the new valie of the property.
        /// </summary>
        public object NewValue { get; private set; }

        /// <summary>
        /// Gets the old value of the property.
        /// </summary>
        public object OldValue { get; private set; }

        /// <summary>
        /// Gets the name of the changed property.
        /// </summary>
        public string PropertyName { get; private set; }
    }
}