using System;

namespace Cauldron
{
    /// <summary>
    /// Specifies that the property, field or constructor contains a type or parameter that can be supplied by the <see cref="Factory"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Constructor | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class InjectAttribute : Attribute
    {
    }
}