using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Cauldron.Core.Interceptors
{
    public interface IPropertyInterceptor
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

        void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value);
    }
}