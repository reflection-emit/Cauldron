namespace Cauldron.Interception
{
    /// <summary>
    /// Represents an interceptor that is forced to initialize when the declaring type is initialized.
    /// </summary>
    public interface IPropertyInterceptorInitialize
    {
        /// <summary>
        /// Invoked if the declaring class is initialized.
        /// </summary>
        /// <param name="propertyInterceptionInfo">
        /// An object that containes information about the intercepted method
        /// </param>
        /// <param name="value">The current value of the property</param>
        void OnInitialize(PropertyInterceptionInfo propertyInterceptionInfo, object value);
    }
}