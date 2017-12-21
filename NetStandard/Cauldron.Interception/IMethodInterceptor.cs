using System;
using System.Reflection;

namespace Cauldron.Interception
{
    /// <summary>
    /// Represents a method interceptor
    /// </summary>
    /// <example>
    /// Sample implementation:
    /// <code>
    /// [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    /// public class MyInterceptorAttribute : Attribute, IMethodInterceptor
    /// {
    ///     public MyInterceptorAttribute()
    ///     {
    ///     }
    ///
    ///     public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
    ///     {
    ///     }
    ///
    ///     public void OnException(Exception e)
    ///     {
    ///     }
    ///
    ///     public void OnExit()
    ///     {
    ///     }
    /// }
    /// </code>
    /// The interceptor is also capable of handling attributes with parameters.
    /// <para/>
    /// Your code:
    /// <code>
    /// public class SampleClass
    /// {
    ///     [MyInterceptor]
    ///     public void SampleMethod()
    ///     {
    ///         Debug.WriteLine("Blablablablablabla");
    ///     }
    /// }
    /// </code>
    /// What gets compiled:
    /// <code>
    /// public class SampleClass
    /// {
    ///     public void SampleMethod()
    ///     {
    ///         var interceptorAttribute = new MyInterceptorAttribute("Any valid attribute parameter types");
    ///
    ///         try
    ///         {
    ///             interceptorAttribute.OnEnter(typeof(SampleClass), this, MethodBase.GetMethodFromHandle(methodof(SampleClass.SampleMethod()).MethodHandle, typeof(SampleClass).TypeHandle), new object[0]);
    ///             Debug.WriteLine("Blablablablablabla");
    ///         }
    ///         catch (Exception e)
    ///         {
    ///             interceptorAttribute.OnException(e);
    ///             throw;
    ///         }
    ///         finally
    ///         {
    ///             interceptorAttribute.OnExit();
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IMethodInterceptor : IInterceptor
    {
        /// <summary>
        /// Invoked if an intercepted method has been called
        /// </summary>
        /// <param name="declaringType">The type declaring the intercepted method</param>
        /// <param name="instance">
        /// The instance of the class where the method is residing. will be null if the method is static
        /// </param>
        /// <param name="methodbase">Contains information about the method</param>
        /// <param name="values">The passed arguments of the method.</param>
        void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values);

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