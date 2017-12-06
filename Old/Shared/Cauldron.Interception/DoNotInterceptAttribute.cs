using System;

namespace Cauldron.Interception
{
    /// <summary>
    /// Indicates that the method or property should not be intercepted. <br/>
    /// ATTENTION: Only the property and method interceptor that is applied to a type respects this attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class DoNotInterceptAttribute : Attribute, IInterceptor
    {
    }
}