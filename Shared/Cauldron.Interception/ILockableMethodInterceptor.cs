using System;
using System.Reflection;
using System.Threading;

namespace Cauldron.Interception
{
    /// <summary>
    /// Represents a method interceptor with a <see cref="SemaphoreSlim"/>
    /// </summary>
    public interface ILockableMethodInterceptor : IInterceptor
    {
        /// <summary>
        /// Invoked if an intercepted method has been called
        /// </summary>
        /// <param name="semaphore">
        /// The <see cref="SemaphoreSlim"/> instance that can be used to lock the the method
        /// </param>
        /// <param name="declaringType">The type declaring the intercepted method</param>
        /// <param name="instance">
        /// The instance of the class where the method is residing. will be null if the method is static
        /// </param>
        /// <param name="methodbase">Contains information about the method</param>
        /// <param name="values">The passed arguments of the method.</param>
        void OnEnter(SemaphoreSlim semaphore, Type declaringType, object instance, MethodBase methodbase, object[] values);

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
    }
}