using Cauldron.Interception;
using NLog;
using System;
using System.Reflection;

namespace Cauldron.Interceptors
{
    /// <summary>
    /// Intercepts the method and logs an exception.
    /// <para/>
    /// This interceptor is using NLog. NLog configuration will affect this interceptor.
    /// </summary>
    [InterceptorOptions(AlwaysCreateNewInstance = true)]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ExceptionLoggerAttribute : Attribute, IMethodInterceptor
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <exclude/>
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
        }

        /// <exclude/>
        public bool OnException(Exception e)
        {
            logger.Log(LogLevel.Error, e);
            return true;
        }

        /// <exclude/>
        public void OnExit()
        {
        }
    }
}