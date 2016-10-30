using Cauldron.Core;
using Cauldron.Core.Extensions;
using Cauldron.XAML.Validation.ViewModels;
using System;
using System.Reflection;

namespace Cauldron.XAML.Validation
{
    /// <summary>
    /// Validates a property for the string length.
    /// <para/>
    /// A <see cref="string"/> that is shorter or longer than the specified length will cause a validation error
    /// </summary>
    /// <exception cref="ArgumentException">If the property is applied to a type that is not a string</exception>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class StringLengthAttribute : ValidationBaseAttribute
    {
        private int maxLength;
        private int minLength;

        /// <summary>
        /// Initializes a new instance of <see cref="StringLengthAttribute"/>
        /// </summary>
        /// <param name="minLength">The minimum length the string should have</param>
        /// <param name="maxLength">The maximum length the string should have</param>
        /// <param name="errorMessageKey">The key of the localized error message string</param>
        public StringLengthAttribute(int minLength, int maxLength, string errorMessageKey) : base(errorMessageKey)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="StringLengthAttribute"/>
        /// </summary>
        /// <param name="length">The length the string should have</param>
        /// <param name="errorMessageKey">The key of the localized error message string</param>
        public StringLengthAttribute(int length, string errorMessageKey) : base(errorMessageKey)
        {
            this.minLength = length;
            this.maxLength = length;
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
            if (propertyInfo.PropertyType != typeof(string))
                throw new ArgumentException("The StringLengthAttribute can only be applied to a string");

            var str = value as string;

            if (str == null)
                return true;

            var length = str.Length;

            return length < this.minLength || length > this.maxLength;
        }

        /// <summary>
        /// Occures on validation
        /// <para/>
        /// Can be used to modify the validation error message.
        /// </summary>
        /// <param name="errorMessage">The validation error message</param>
        /// <param name="context">The Viewmodel context that has to be validated</param>
        /// <returns>A modified validation error message</returns>
        protected override string ValidationMessage(string errorMessage, IValidatableViewModel context) => errorMessage.ToString(minLength, maxLength);
    }
}