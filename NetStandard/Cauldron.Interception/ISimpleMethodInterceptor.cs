using System;
using System.Reflection;

namespace Cauldron.Interception
{
    /// <summary>
    /// Represents a simple method interceptor
    /// </summary>
    /// <example>
    /// Sample implementation:
    /// <code>
    /// [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    /// public class MyInterceptorAttribute : Attribute, ISimpleMethodInterceptor
    /// {
    ///     public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
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
    ///         interceptorAttribute.OnEnter(typeof(SampleClass), this, MethodBase.GetMethodFromHandle(methodof(SampleClass.SampleMethod()).MethodHandle, typeof(SampleClass).TypeHandle), new object[0]);
    ///         Debug.WriteLine("Blablablablablabla");
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface ISimpleMethodInterceptor : IInterceptor
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
    }
}