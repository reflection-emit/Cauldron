using Cauldron.Activator;
using System;

namespace Cauldron.Injection
{
    /// <summary>
    /// Specifies that the property, field or argument contains a type or parameter that can be supplied by the <see cref="Factory"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class InjectAttribute : Attribute
    {
    }
}