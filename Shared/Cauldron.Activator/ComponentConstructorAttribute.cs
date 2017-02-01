using System;

namespace Cauldron.Activator
{
    /// <summary>
    /// Specifies that <see cref="Type"/> can be constructed by using this particular Constructor or Method
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ComponentConstructorAttribute : Attribute
    {
    }
}