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
        void OnException(Exception e);

        /// <summary>
        /// Invoked if the intercepted constructor has finished executing.
        /// </summary>
        void OnExit();
    }
}