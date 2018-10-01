using Cauldron.Cryptography;
using Cauldron.XAML.ViewModels;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Cauldron.XAML.Validation
{
    /// <summary>
    /// Validates the password strength the property.
    /// <para/>
    /// <see cref="PasswordScore.Blank"/>, <see cref="PasswordScore.VeryWeak"/> and <see
    /// cref="PasswordScore.Weak"/> will cause a validation error
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class PasswordStrengthAttribute : ValidatorAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PasswordStrengthAttribute"/>
        /// </summary>
        /// <param name="errorMessageKey">The key of the localized error message string</param>
        public PasswordStrengthAttribute(string errorMessageKey) : base(errorMessageKey)
        {
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
            var strength = PasswordScore.Blank;
            var password = string.Empty;

            if (propertyInfo.PropertyType == typeof(string))
                strength = CryptoUtils.GetPasswordScore(password);

            return Task.FromResult(strength == PasswordScore.Blank || strength == PasswordScore.VeryWeak || strength == PasswordScore.Weak);
        }
    }
}