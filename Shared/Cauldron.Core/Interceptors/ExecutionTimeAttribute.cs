using Cauldron.Interception;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Cauldron.Core.Interceptors
{
    /// <summary>
    /// Provides a simple performance measurement of a code block
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class ExecutionTimeAttribute : Attribute, IMethodInterceptor, IPropertyInterceptor
    {
        private string memberName;

        private Stopwatch startTime;

        /// <summary>
        /// Initializes a new instance of <see cref="ExecutionTimeAttribute"/>
        /// </summary>
        public ExecutionTimeAttribute()
        {
            this.startTime = Stopwatch.StartNew();
        }

        /// <summary>
        /// Invoked if an intercepted method has been called
        /// </summary>
        /// <param name="declaringType">The type declaring the intercepted method</param>
        /// <param name="instance">The instance of the class where the method is residing. will be null if the method is static</param>
        /// <param name="methodbase">Contains information about the method</param>
        /// <param name="values">The passed arguments of the method.</param>
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            this.memberName = methodbase.DeclaringType.FullName + "." + methodbase.Name;
            Output.WriteLineInfo("Start execution of '{0}'", this.memberName);
        }

        /// <summary>
        /// Invoked if an intercepted method has raised an exception. The method will always rethrow the exception.
        /// </summary>
        /// <param name="e">The exception information.</param>
        public void OnException(Exception e)
        {
        }

        /// <summary>
        /// Invoked if the intercepted method has finished executing.
        /// </summary>
        public void OnExit()
        {
            this.startTime.Stop();
            Output.WriteLineInfo("Execution of '{0}': {1}ms", this.memberName, this.startTime.Elapsed.TotalMilliseconds);
        }

        /// <summary>
        /// Invoked if the intercepted property getter has been called
        /// </summary>
        /// <param name="propertyInterceptionInfo">An object that containes information about the intercepted method</param>
        /// <param name="value">The current value of the property</param>
        public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
            this.memberName = propertyInterceptionInfo.DeclaringType.FullName + "." + propertyInterceptionInfo.PropertyName;
            Output.WriteLineInfo("Start execution of '{0}'", this.memberName);
        }

        /// <summary>
        /// Invoked if the intercepted property setter has been called
        /// </summary>
        /// <param name="propertyInterceptionInfo">An object that containes information about the intercepted method</param>
        /// <param name="oldValue">The current value of the property</param>
        /// <param name="newValue">The to be new value of the property</param>
        /// <returns>If returns false, the backing field will be set to <paramref name="newValue"/></returns>
        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            this.memberName = propertyInterceptionInfo.DeclaringType.FullName + "." + propertyInterceptionInfo.PropertyName;
            Output.WriteLineInfo("Start execution of '{0}'", this.memberName);
            return false;
        }
    }
}