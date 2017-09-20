using System;
using System.Threading;

namespace Cauldron.Interception
{
    /// <summary>
    /// Represents an interceptor that can intercept a property's setter method with a <see cref="SemaphoreSlim"/>
    /// </summary>
    public interface ILockablePropertySetterInterceptor : IInterceptor
    {
        /// <summary>
        /// Invoked if an intercepted method has raised an exception. The method will always rethrow
        /// the exception.
        /// </summary>
        /// <param name="e">The exception information.</param>
        void OnException(Exception e);

        /// <summary>
        /// Invoked if the intercepted method has finished executing.
        /// </summary>
        void OnExit();

        /// <summary>
        /// Invoked if the intercepted property setter has been called
        /// </summary>
        /// <param name="semaphore">
        /// The <see cref="SemaphoreSlim"/> instance that can be used to lock the the method
        /// </param>
        /// <param name="propertyInterceptionInfo">
        /// An object that containes information about the intercepted method
        /// </param>
        /// <param name="oldValue">The current value of the property</param>
        /// <param name="newValue">The to be new value of the property</param>
        /// <returns>If returns false, the backing field will be set to <paramref name="newValue"/></returns>
        bool OnSet(SemaphoreSlim semaphore, PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue);
    }
}