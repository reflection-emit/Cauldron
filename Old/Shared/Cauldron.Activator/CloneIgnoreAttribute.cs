using System;

namespace Cauldron.Activator
{
    /// <summary>
    /// Instructs the Cloner not to clone the public field or public read/write property value
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class CloneIgnoreAttribute : Attribute
    {
    }
}