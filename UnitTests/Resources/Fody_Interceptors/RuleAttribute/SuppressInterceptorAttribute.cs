using System;

namespace Cauldron.UnitTest.Interceptors.Property.RuleAttribute
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class SuppressInterceptorAttribute : Attribute
    {
    }
}