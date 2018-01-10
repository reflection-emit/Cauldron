using System;
using System.Reflection;

namespace Cauldron.Interception
{
    /// <summary>
    /// Represents a contructor interceptor.
    /// <para/>
    /// Please note that <see cref="ISyncRoot"/> is NOT supported by this interceptor.
    /// </summary>
    /// <example>
    /// Sample implementation:
    /// <code>
    /// [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    /// public sealed class TestConstructorInterceptorA : Attribute, IConstructorInterceptor
    /// {
    ///     public void OnBeforeInitialization(Type declaringType, MethodBase methodbase, object[] values)
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
    /// public class ConstructorInterceptorTestClass
    /// {
    ///     [TestConstructorInterceptorA]
    ///     public ConstructorInterceptorTestClass(string arg)
    ///     {
    ///     }
    /// }
    /// </code>
    /// What gets compiled:
    /// <code>
    /// public ConstructorInterceptorTestClass(string arg)
    /// {
    ///     var values = new object[] { arg };
    ///     var constructorInterceptor = new TestConstructorInterceptorA();
    ///     constructorInterceptor.OnBeforeInitialization(typeof(ConstructorInterceptorTestClass), MethodBase.GetMethodFromHandle(methodof(ConstructorInterceptorTestClass..ctor()).MethodHandle, typeof(ConstructorInterceptorTestClass).TypeHandle), values);
    ///     base..ctor();
    ///     try
    ///     {
    ///	        constructorInterceptor.OnEnter(typeof(ConstructorInterceptorTestClass), this, MethodBase.GetMethodFromHandle(methodof(ConstructorInterceptorTestClass..ctor()).MethodHandle, typeof(ConstructorInterceptorTestClass).TypeHandle), values);
    ///     }
    ///     catch (Exception e)
    ///     {
    ///	        constructorInterceptor.OnException(e);
    ///	        throw;
    ///     }
    ///     finally
    ///     {
    ///	        constructorInterceptor.OnExit();
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IConstructorInterceptor : IInterceptor
    {
        /// <summary>
        /// Invoked before the initialization of the class.
        /// <para/>
        /// This happens before the base call which means that the instance has not been created yet. Use this with care.
        /// </summary>
        /// <param name="declaringType">The type declaring the intercepted contructor</param>
        /// <param name="methodbase">Contains information about the constructor.</param>
        /// <param name="values">The passed arguments of the constructor.</param>
        void OnBeforeInitialization(Type declaringType, MethodBase methodbase, object[] values);

        /// <summary>
        /// Invoked if an intercepted contructor has been called.
        /// </summary>
        /// <param name="declaringType">The type declaring the intercepted contructor</param>
        /// <param name="instance">
        /// The instance of the class where the method is residing. will be null if the constructor is static.
        /// </param>
        /// <param name="methodbase">Contains information about the constructor.</param>
        /// <param name="values">The passed arguments of the constructor.</param>
        void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values);

        /// <summary>
        /// Invoked if an intercepted construtor has raised an exception. The constructor will always rethrow
        /// the exception.
        /// </summary>
        /// <param name="e">The exception information.</param>
        /// <returns>Should return true if the exception should be rethrown; otherwise false</returns>
        bool OnException(Exception e);

        /// <summary>
        /// Invoked if the intercepted constructor has finished executing.
        /// </summary>
        void OnExit();
    }
}