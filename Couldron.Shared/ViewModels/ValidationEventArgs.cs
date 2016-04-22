using System;

namespace Cauldron.ViewModels
{
    /// <summary>
    /// Provides data for the <see cref="ValidationHandler.Validation"/> event.
    /// </summary>
    public sealed class ValidationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationEventArgs"/> class
        /// </summary>
        /// <param name="propertyName"></param>
        public ValidationEventArgs(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationEventArgs"/> class
        /// </summary>
        public ValidationEventArgs() : this("")
        {
        }

        /// <summary>
        /// Gets the property name of the property that need to be validated.
        /// <para/>
        /// The value is empty if a properties are validated
        /// </summary>
        public string PropertyName { get; private set; }
    }
}