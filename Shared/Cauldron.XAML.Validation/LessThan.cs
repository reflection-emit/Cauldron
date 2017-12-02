using Cauldron.Core;
using Cauldron.Core.Extensions;
using Cauldron.XAML.Validation.ViewModels;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Cauldron.XAML.Validation
{
    /// <summary>
    /// Validates the property if value is less than the given value or the given property
    /// </summary>
    /// <exception cref="ArgumentException">The property was not found</exception>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class LessThanAttribute : ValidatorAttributeBase
    {
        private string propertyName;
        private object value;

        /// <summary>
        /// Initializes a new instance of <see cref="LessThanAttribute"/>
        /// </summary>
        /// <param name="value">The value to compare to</param>
        /// <param name="errorMessageKey">The key of the localized error message string</param>
        public LessThanAttribute(object value, string errorMessageKey) : base(errorMessageKey)
        {
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LessThanAttribute"/>
        /// </summary>
        /// <param name="propertyName">The name of the property to validate against</param>
        /// <param name="errorMessageKey">The key of the localized error message string</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is null</exception>
        public LessThanAttribute(string propertyName, string errorMessageKey) : base(errorMessageKey)
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
        protected override Task<bool> OnValidateAsync(PropertyInfo sender, IValidatableViewModel context, PropertyInfo propertyInfo, object value)
        {
            // if the value is null, then we should return a validation successfull, so that it is
            // possible to have non mandatory inputs
            if (value == null)
                return Task.FromResult(false);

            if (this.value != null)
                return Task.FromResult(!ComparerUtils.LessThan(value, this.value));

            var otherProperty = context.GetType().GetPropertyEx(this.propertyName);

            if (otherProperty == null)
                throw new ArgumentException(string.Format("The property '{0}' was not found on '{1}'.", this.propertyName, context.GetType().FullName));

            // Also same for this value
            var comparisonValue = otherProperty.GetValue(context);

            if (comparisonValue == null)
                return Task.FromResult(false);

            return Task.FromResult(!ComparerUtils.LessThan(value, comparisonValue));
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
                return errorMessage.ToString(this.value);

            var otherProperty = context.GetType().GetPropertyEx(this.propertyName);

            if (otherProperty == null)
                return errorMessage;

            return errorMessage.ToString(otherProperty.GetValue(context));
        }
    }
}