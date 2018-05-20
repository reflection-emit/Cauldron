using System;
using System.Reflection;

namespace Cauldron.Interception
{
    /// <summary>
    /// Represents a method interceptor. This interceptor can also be applied to abstract methods and all overriding methods will implement this interceptor.
    /// </summary>
    /// <example>
    /// Sample implementation:
    /// <code>
    /// [InterceptorOptions(AlwaysCreateNewInstance = true)]
    /// [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    /// public class MyInterceptorAttribute : Attribute, IMethodInterceptor
    /// {
    ///     public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
    ///     {
    ///     }
    ///
    ///     public bool OnException(Exception e)
    ///     {
    ///         // Returning false will swallow the exception
    ///         return true;
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
    ///         var interceptorAttribute = new MyInterceptorAttribute();
    ///
    ///         try
    ///         {
    ///             interceptorAttribute.OnEnter(typeof(SampleClass), this, MethodBase.GetMethodFromHandle(methodof(SampleClass.SampleMethod()).MethodHandle, typeof(SampleClass).TypeHandle), new object[0]);
    ///             Debug.WriteLine("Blablablablablabla");
    ///         }
    ///         catch (Exception e)
    ///         {
    ///             if(interceptorAttribute.OnException(e))
    ///             {
    ///                 throw;
    ///             }
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
        /// Invoked if an intercepted method has raised an exception.
        /// </summary>
        /// <param name="e">The exception information.</param>
        /// <returns>Should return true if the exception should be rethrown; otherwise false</returns>
        bool OnException(Exception e);

        /// <summary>
        /// Invoked if the intercepted method has finished executing.
        /// </summary>
        void OnExit();
    }
}