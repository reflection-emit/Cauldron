using System;

namespace Cauldron.Core.Interceptors
{
    /// <summary>
    /// Represents an interceptor that can intercept a property's getter method
    /// </summary>
    public interface IPropertyGetterInterceptor
    {
        /// <summary>
        /// Invoked if an intercepted method has raised an exception. The method will always rethrow the exception.
        /// </summary>
        /// <param name="e">The exception information.</param>
        void OnException(Exception e);

        /// <summary>
        /// Invoked if the intercepted method has finished executing.
        /// </summary>
        void OnExit();

        /// <summary>
        /// Invoked if the intercepted property getter has been called
        /// </summary>
        /// <param name="propertyInterceptionInfo">An object that containes information about the intercepted method</param>
        /// <param name="value">The current value of the property</param>
        void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value);
    }
}