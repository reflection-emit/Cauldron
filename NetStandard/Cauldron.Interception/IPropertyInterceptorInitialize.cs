namespace Cauldron.Interception
{
    /// <summary>
    /// Moves the property interceptor's initialization from 'first use' to the constructor of the declaring type.
    /// <para/>
    /// This interceptor extension is available for <see cref="IPropertyGetterInterceptor"/>, <see cref="IPropertySetterInterceptor"/> and <see cref="IPropertyInterceptor"/>.
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