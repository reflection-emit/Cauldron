using Cauldron.XAML.Validation.ViewModels;
using System;

namespace Cauldron.XAML.Validation
{
    /// <summary>
    /// Provides event data to the <see cref="IValidatableViewModel.Validating"/> event.
    /// </summary>
    public sealed class ValidationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ValidationEventArgs"/>.
        /// </summary>
        /// <param name="propertyName">The property name of the property that is being validated.</param>
        public ValidationEventArgs(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        /// <summary>
        /// Gets the property name of the property that is being validated.
        /// </summary>
        public string PropertyName { get; private set; }
    }
}