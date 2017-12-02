namespace Cauldron.XAML.Validation
{
    /// <summary>
    /// Handles validation of a viewmodel
    /// </summary>
    public static class ValidationHandler
    {
        /// <summary>
        /// Gets or set a value that indicates if <see cref="ValidatorAttributeBase.AlwaysValidate"/>
        /// should be ignored.
        /// <para/>
        /// Default value is false.
        /// </summary>
        public static bool IgnoreAlwaysValidate { get; set; } = false;

        /// <summary>
        /// Gets or sets a value that indicates if all validators are executed.
        /// <para/>
        /// Default value is false.
        /// <para/>
        /// The default behaviour of the <see cref="ValidationHandler"/> is to stop validation on
        /// first error.
        /// </summary>
        public static bool StopValidationOnError { get; set; } = false;
    }
}