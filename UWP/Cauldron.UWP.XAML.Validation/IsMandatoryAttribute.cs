using Cauldron.IEnumerableExtensions;
using Cauldron.XAML.Validation.ViewModels;
using System;
using System.Collections;
using System.Reflection;

namespace Cauldron.XAML.Validation
{
    /// <summary>
    /// Validates a property for mandatory value.
    /// <para/>
    /// Value is null or empty will cause a validation error
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class IsMandatoryAttribute : ValidationBaseAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IsMandatoryAttribute"/>
        /// </summary>
        /// <param name="errorMessageKey">The key of the localized error message string</param>
        public IsMandatoryAttribute(string errorMessageKey) : base(errorMessageKey)
        {
        }

        /// <summary>
        /// Gets a value that if true indicates that this validator will be invoked everytime the property has changed
        /// </summary>
        public override bool AlwaysValidate { get { return false; } }

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
            if (value == null)
                return true;

            if (propertyInfo.PropertyType == typeof(string))
                return string.IsNullOrEmpty(value.ToString());

            if (propertyInfo.PropertyType == typeof(bool))
                return !(bool)value;

            if (value is IEnumerable && (value as IEnumerable).Count_() == 0)
                return true;

            return false;
        }
    }
}