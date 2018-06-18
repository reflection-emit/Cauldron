using System;

namespace Cauldron.Interception
{
    /// <summary>
    /// This extension adds an additional <see cref="OnExit(Type, object)"/> method, which allows the interceptor to read and modify the return value of the method.
    /// Adding this interface to the <see cref="IMethodInterceptor"/> will render the <see cref="IMethodInterceptor.OnExit"/> useless.
    /// <para/>
    /// This interceptor extension is only available for the <see cref="IMethodInterceptor"/>.
    /// </summary>
    public interface IMethodInterceptorOnExit
    {
        /// <summary>
        /// Invoked if the intercepted method has finished executing. Renders the <see cref="IMethodInterceptor.OnExit"/> useless.
        /// If the <paramref name="returnType"/> is a <see cref="void"/>, then <paramref name="returnValue"/> is null.
        /// </summary>
        /// <example>
        /// <code>
        /// public object OnExit(Type returnType, object returnValue)
        /// {
        ///     return returnValue;
        /// }
        /// </code>
        /// </example>
        /// <param name="returnType">The return type of the method.</param>
        /// <param name="returnValue">The return value of the method.</param>
        /// <returns>The return value of the method.</returns>
        object OnExit(Type returnType, object returnValue);
    }
}