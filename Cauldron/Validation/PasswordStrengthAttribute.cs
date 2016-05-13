using Cauldron.Core;
using Cauldron.ViewModels;
using System;
using System.Reflection;
using System.Security;

namespace Cauldron.Validation
{
    /// <summary>
    /// Validates the password strength the property.
    /// <para/>
    /// <see cref="PasswordScore.Blank"/>, <see cref="PasswordScore.VeryWeak"/> and <see cref="PasswordScore.Weak"/> will cause a validation error
    /// <para/>
    /// ATTENTION: Using this <see cref="Attribute"/> against a <see cref="SecureString"/> may jeopardize security
    /// </summary>
    public sealed partial class PasswordStrengthAttribute : ValidationBaseAttribute
    {
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
            var strength = PasswordScore.Blank;
            var password = string.Empty;

            if (propertyInfo.PropertyType == typeof(string))
                strength = Utils.Current.GetPasswordScore(password);
            else if (propertyInfo.PropertyType == typeof(SecureString))
                using (var secureString = new SecureStringHandler(value as SecureString))
                {
                    strength = Utils.Current.GetPasswordScore(secureString.ToString());
                }

            return strength == PasswordScore.Blank || strength == PasswordScore.VeryWeak || strength == PasswordScore.Weak;
        }
    }
}