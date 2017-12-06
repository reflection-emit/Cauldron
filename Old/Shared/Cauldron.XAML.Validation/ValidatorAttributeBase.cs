using Cauldron.XAML.Validation.ViewModels;
using System;
using Cauldron.Localization;
using System.Reflection;
using System.Threading.Tasks;

namespace Cauldron.XAML.Validation
{
    /// <summary>
    /// Specifies the validation method for a property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class ValidatorAttributeBase : Attribute
    {
        internal PropertyInfo propertyInfo = null;

        /// <summary>
        /// Initializes a new instance of <see cref="ValidatorAttributeBase"/>
        /// </summary>
        /// <param name="errorMessageKey">The key of the localized error message string</param>
        protected ValidatorAttributeBase(string errorMessageKey)
        {
            this.ErrorMessageKey = errorMessageKey;
        }

        /// <summary>
        /// Gets a value that if true indicates that this validator will be invoked everytime the
        /// property has changed
        /// </summary>
        public virtual bool AlwaysValidate { get { return true; } }

        /// <summary>
        /// Gets the key of the localized error message string
        /// </summary>
        public string ErrorMessageKey { get; private set; }

        /// <summary>
        /// Invokes the validation on the specified context with the specified parameters
        /// </summary>
        /// <param name="sender">The property that invoked the validation</param>
        /// <param name="context">The Viewmodel context that has to be validated</param>
        /// <returns>Returns the error message if validation fails. Empty if successful</returns>
        internal async Task<string> ValidateAsync(PropertyInfo sender, IValidatableViewModel context)
        {
            var value = this.propertyInfo.GetValue(context);

            if (await this.OnValidateAsync(sender, context, propertyInfo, value))
                return this.ValidationMessage(Locale.Current[this.ErrorMessageKey], context);

            return string.Empty;
        }

        /// <summary>
        /// Invokes the validation on the specified context with the specified parameters
        /// </summary>
        /// <param name="sender">The property that invoked the validation</param>
        /// <param name="context">The Viewmodel context that has to be validated</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> of the validated property</param>
        /// <param name="value">The value of the property</param>
        /// <returns>Has to return true on validation error otherwise false</returns>
        protected abstract Task<bool> OnValidateAsync(PropertyInfo sender, IValidatableViewModel context, PropertyInfo propertyInfo, object value);

        /// <summary>
        /// Occures on validation
        /// <para/>
        /// Can be used to modify the validation error message.
        /// </summary>
        /// <param name="errorMessage">The validation error message</param>
        /// <param name="context">The Viewmodel context that has to be validated</param>
        /// <returns>A modified validation error message</returns>
        protected virtual string ValidationMessage(string errorMessage, IValidatableViewModel context) => errorMessage;
    }
}