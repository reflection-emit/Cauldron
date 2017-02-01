using Cauldron.Activator;
using Cauldron.Localization;
using Cauldron.XAML.Validation.ViewModels;
using System;
using System.Reflection;

namespace Cauldron.XAML.Validation
{
    /// <summary>
    /// Specifies the validation method for a property
    /// </summary>
    public abstract class ValidationBaseAttribute : Attribute
    {
        internal PropertyInfo propertyInfo;
        private Locale localization;

        /// <summary>
        /// Initializes a new instance of <see cref="ValidationBaseAttribute"/>
        /// </summary>
        /// <param name="errorMessageKey">The key of the localized error message string</param>
        public ValidationBaseAttribute(string errorMessageKey)
        {
            this.ErrorMessageKey = errorMessageKey;

            if (Factory.HasContract(typeof(ILocalizationSource)))
                this.localization = Factory.Create<Locale>();
        }

        /// <summary>
        /// Gets a value that if true indicates that this validator will be invoked everytime the property has changed
        /// </summary>
        public virtual bool AlwaysValidate { get { return true; } }

        /// <summary>
        /// Gets or sets the key of the localized error message string
        /// </summary>
        public string ErrorMessageKey { get; private set; }

        /// <summary>
        /// Invokes the validation on the specified context with the specified parameters
        /// </summary>
        /// <param name="sender">The property that invoked the validation</param>
        /// <param name="context">The Viewmodel context that has to be validated</param>
        /// <returns>Returns the error message if validation fails. Empty if successful</returns>
        internal string Validate(PropertyInfo sender, IValidatableViewModel context)
        {
            object value = this.propertyInfo.GetValue(context);

            if (this.OnValidate(sender, context, propertyInfo, value))
                return this.ValidationMessage(this.localization == null ? this.ErrorMessageKey : this.localization[this.ErrorMessageKey], context);

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
        protected abstract bool OnValidate(PropertyInfo sender, IValidatableViewModel context, PropertyInfo propertyInfo, object value);

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