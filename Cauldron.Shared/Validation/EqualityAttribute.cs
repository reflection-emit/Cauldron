using Cauldron.ViewModels;
using System;
using System.Reflection;

namespace Cauldron.Validation
{
    /// <summary>
    /// Validates if two properties are equal in value
    /// <para/>
    /// Causes a validation error if the values of both properties are not equal
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class EqualityAttribute : ValidationBaseAttribute
    {
        private string otherProperty;

        /// <summary>
        /// Initializes a new instance of <see cref="EqualityAttribute"/>
        /// </summary>
        /// <param name="otherProperty">The property name of the property this property has to be equal to</param>
        /// <param name="errorMessageKey">The key of the localized error message string</param>
        public EqualityAttribute(string otherProperty, string errorMessageKey) : base(errorMessageKey)
        {
            this.otherProperty = otherProperty;
        }

        /// <summary>
        /// Invokes the validation on the specified context with the specified parameters
        /// </summary>
        /// <param name="sender">The property that invoked the validation</param>
        /// <param name="context">The viewmodel context that has to be validated</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> of the validated property</param>
        /// <param name="value">The value of the property</param>
        /// <returns>Has to return true on validation error otherwise false</returns>
        protected override bool OnValidate(PropertyInfo sender, IValidatableViewModel context, PropertyInfo propertyInfo, object value)
        {
            // if the value is null, then we should return a validation successfull, so that it is possible
            // to have non mandatory inputs
            if (value == null)
                return false;

            var secondProperty = context.GetType().GetProperty(this.otherProperty);

            if (secondProperty == null)
                return true;

            var secondValue = secondProperty.GetValue(context);

            if (sender == null)
                context.Validate(propertyInfo, secondProperty.Name);

            return !Utils.Current.Equals(value, secondValue);
        }

        /// <summary>
        /// Occures on validation
        /// <para/>
        /// Can be used to modify the validation error message.
        /// </summary>
        /// <param name="errorMessage">The validtion error message</param>
        /// <param name="context">The Viewmodel context that has to be validated</param>
        /// <returns>A modified validation error message</returns>
        protected override string ValidationMessage(string errorMessage, IValidatableViewModel context)
        {
            var otherProperty = context.GetType().GetProperty(this.otherProperty);

            if (otherProperty == null)
                return errorMessage;

            return string.Format(errorMessage, otherProperty.GetValue(context));
        }
    }
}