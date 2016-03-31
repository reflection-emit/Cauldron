using System;

namespace Couldron.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed partial class PasswordStrengthAttribute : ValidationBaseAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PasswordStrengthAttribute"/>
        /// </summary>
        /// <param name="errorMessageKey">The key of the localized error message string</param>
        public PasswordStrengthAttribute(string errorMessageKey) : base(errorMessageKey)
        {
        }
    }
}