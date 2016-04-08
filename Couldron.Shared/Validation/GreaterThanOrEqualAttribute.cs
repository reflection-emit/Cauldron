using Couldron.ViewModels;
using System;
using System.Reflection;

namespace Couldron.Validation
{
    /// <summary>
    /// Validates the property if value is greater than or equal the given value or the given property
    /// </summary>
    /// <exception cref="ArgumentException">The property was not found</exception>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class GreaterThanOrEqualAttribute : ValidationBaseAttribute
    {
        private string propertyName;
        private object value;

        /// <summary>
        /// Initializes a new instance of <see cref="GreaterThanOrEqualAttribute"/>
        /// </summary>
        /// <param name="value">The value to compare to</param>
        /// <param name="errorMessageKey">The key of the localized error message string</param>
        public GreaterThanOrEqualAttribute(object value, string errorMessageKey) : base(errorMessageKey)
        {
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GreaterThanOrEqualAttribute"/>
        /// </summary>
        /// <param name="propertyName">The name of the property to validate against</param>
        /// <param name="errorMessageKey">The key of the localized error message string</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is null</exception>
        public GreaterThanOrEqualAttribute(string propertyName, string errorMessageKey) : base(errorMessageKey)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            this.propertyName = propertyName;
        }

        /// <summary>
        /// Invokes the validation on the specified context with the specified parameters
        /// </summary>
        /// <param name="sender">The property that invoked the validation</param>
        /// <param name="context">The Viewmodel context that has to be validated</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> of the validated property</param>
        /// <param name="value">The value of the property</param>
        /// <returns>Has to return true on validation error otherwise false</returns>
        protected override bool OnValidate(PropertyInfo sender, IValidatableViewModel context, PropertyInfo propertyInfo, object value)
        {
            // if the value is null, then we should return a validation successfull, so that it is possible
            // to have non mandatory inputs
            if (value == null)
                return false;

            if (this.value != null)
                return !Utils.GreaterThanOrEqual(this.value, value);

            var otherProperty = context.GetType().GetProperty(this.propertyName);

            if (otherProperty == null)
                throw new ArgumentException(string.Format("The property '{0}' was not found on '{1}'.", this.propertyName, context.GetType().FullName));

            return !Utils.GreaterThanOrEqual(value, otherProperty.GetValue(context));
        }

        /// <summary>
        /// Occures on validation
        /// <para/>
        /// Can be used to modify the validation error message.
        /// </summary>
        /// <param name="errorMessage">The validation error message</param>
        /// <param name="context">The Viewmodel context that has to be validated</param>
        /// <returns>A modified validation error message</returns>
        protected override string ValidationMessage(string errorMessage, IValidatableViewModel context)
        {
            if (this.value != null)
                return string.Format(errorMessage, this.value);

            var otherProperty = context.GetType().GetProperty(this.propertyName);

            if (otherProperty == null)
                return errorMessage;

            return string.Format(errorMessage, otherProperty.GetValue(context));
        }
    }
}