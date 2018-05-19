using System;

namespace Cauldron.Interception
{
    /// <summary>
    /// Adds additional options to the interceptor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class InterceptorOptionsAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="InterceptorOptionsAttribute"/>.
        /// </summary>
        public InterceptorOptionsAttribute()
        {
        }

        /// <summary>
        /// Gets or sets a value that indicates if a new instance of the interceptor attribute is created every method or property call or not.
        /// <para/>
        /// If false the interceptor instance will be reused for every call, those maintaining its instance; otherwise a new instance is created.
        /// <para/>
        /// This option is available for <see cref="IPropertyGetterInterceptor"/>, <see cref="IPropertySetterInterceptor"/>,
        /// <see cref="IPropertyInterceptor"/>, <see cref="IMethodInterceptor"/> and <see cref="ISimpleMethodInterceptor"/>.
        /// This has no effect if the interceptor is implementing the <see cref="IPropertyInterceptorInitialize"/> interface.
        /// <para/>
        /// The default value is false.
        /// </summary>
        public bool AlwaysCreateNewInstance { get; set; } = false;
    }
}