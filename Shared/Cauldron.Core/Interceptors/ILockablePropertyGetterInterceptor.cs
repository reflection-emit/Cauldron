using System;
using System.Threading;

namespace Cauldron.Core.Interceptors
{
    /// <summary>
    /// Represents an interceptor that can intercept a property's getter method with a <see cref="SemaphoreSlim"/>
    /// </summary>
    public interface ILockablePropertyGetterInterceptor
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
        /// <param name="semaphore">The <see cref="SemaphoreSlim"/> instance that can be used to lock the the method</param>
        /// <param name="propertyInterceptionInfo">An object that containes information about the intercepted method</param>
        /// <param name="value">The current value of the property</param>
        void OnGet(SemaphoreSlim semaphore, PropertyInterceptionInfo propertyInterceptionInfo, object value);
    }
}