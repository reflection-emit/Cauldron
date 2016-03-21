using System;

namespace Couldron.Core
{
    /// <summary>
    /// Specifies that a property with an implemented <see cref="IDisposable"/> interface should not be disposed
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class IgnoreDisposeAttribute : Attribute
    {
    }
}