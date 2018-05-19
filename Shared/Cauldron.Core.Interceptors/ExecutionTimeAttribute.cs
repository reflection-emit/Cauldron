using Cauldron.Interception;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Cauldron.Core.Interceptors
{
    using Cauldron.Core.Diagnostics;

    /// <summary>
    /// Provides a simple performance measurement of a code block
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    [InterceptorOptions(AlwaysCreateNewInstance = true)]
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

        /// <exclude/>
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            this.memberName = methodbase.DeclaringType.FullName + "." + methodbase.Name;
            Debug.WriteLine("Start execution of '{0}'", this.memberName);
        }

        /// <exclude/>
        public bool OnException(Exception e) => true;

        /// <exclude/>
        public void OnExit()
        {
            this.startTime.Stop();
            Debug.WriteLine("Execution of '{0}': {1}ms", this.memberName, this.startTime.Elapsed.TotalMilliseconds);
        }

        /// <exclude/>
        public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
            this.memberName = propertyInterceptionInfo.DeclaringType.FullName + "." + propertyInterceptionInfo.PropertyName;
            Debug.WriteLine("Start execution of '{0}'", this.memberName);
        }

        /// <exclude/>
        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            this.memberName = propertyInterceptionInfo.DeclaringType.FullName + "." + propertyInterceptionInfo.PropertyName;
            Debug.WriteLine("Start execution of '{0}'", this.memberName);
            return false;
        }
    }
}