using Couldron.Core;
using Couldron.ViewModels;
using System.Reflection;

namespace Couldron.Validation
{
    /// <summary>
    /// Validates the password strength the property.
    /// <para/>
    /// <see cref="PasswordScore.Blank"/>, <see cref="PasswordScore.VeryWeak"/> and <see cref="PasswordScore.Weak"/> will cause a validation error
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
                strength = Utils.GetPasswordScore(password);

            return strength == PasswordScore.Blank || strength == PasswordScore.VeryWeak || strength == PasswordScore.Weak;
        }
    }
}