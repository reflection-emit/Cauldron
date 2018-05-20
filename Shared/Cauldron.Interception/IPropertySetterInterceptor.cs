using System;

namespace Cauldron.Interception
{
    /// <summary>
    /// Represents an interceptor that can intercept a property's setter method
    /// </summary>
    public interface IPropertySetterInterceptor : IInterceptor
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
        /// Invoked if the intercepted property setter has been called
        /// </summary>
        /// <param name="propertyInterceptionInfo">
        /// An object that containes information about the intercepted method
        /// </param>
        /// <param name="oldValue">The current value of the property</param>
        /// <param name="newValue">The to be new value of the property</param>
        /// <returns>If returns false, the backing field will be set to <paramref name="newValue"/></returns>
        bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue);
    }
}