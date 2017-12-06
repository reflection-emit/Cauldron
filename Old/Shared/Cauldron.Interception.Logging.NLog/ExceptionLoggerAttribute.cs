using NLog;
using System;
using System.Reflection;

namespace Cauldron.Interception.Logging.NLog
{
    /// <summary>
    /// Intercepts the method and logs an exception
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ExceptionLoggerAttribute : Attribute, IMethodInterceptor
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Invoked if an intercepted method has been called
        /// </summary>
        /// <param name="declaringType">The type declaring the intercepted method</param>
        /// <param name="instance">The instance of the class where the method is residing. will be null if the method is static</param>
        /// <param name="methodbase">Contains information about the method</param>
        /// <param name="values">The passed arguments of the method.</param>
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
        }

        /// <summary>
        /// Invoked if an intercepted method has raised an exception. The method will always rethrow the exception.
        /// </summary>
        /// <param name="e">The exception information.</param>
        public void OnException(Exception e) => logger.Log(LogLevel.Error, e);

        /// <summary>
        /// Invoked if the intercepted method has finished executing.
        /// </summary>
        public void OnExit()
        {
        }
    }
}