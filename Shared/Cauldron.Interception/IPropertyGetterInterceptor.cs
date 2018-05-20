using System;

namespace Cauldron.Interception
{
    /// <summary>
    /// Represents an interceptor that can intercept a property's getter method
    /// </summary>
    public interface IPropertyGetterInterceptor : IInterceptor
    {
        /// <summary>
        /// Invoked if an intercepted method has raised an exception.
        /// </summary>
        /// <param name="e">The exception information.</param>
        /// <returns>Should return true if the exception should be rethrown; otherwise false</returns>
        bool OnException(Exception e);

        /// <summary>
        /// Invoked if the intercepted method has finished executing.
        /// </summary>
        void OnExit();

        /// <summary>
        /// Invoked if the intercepted property getter has been called
        /// </summary>
        /// <param name="propertyInterceptionInfo">
        /// An object that containes information about the intercepted method
        /// </param>
        /// <param name="value">The current value of the property</param>
        void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value);
    }
}