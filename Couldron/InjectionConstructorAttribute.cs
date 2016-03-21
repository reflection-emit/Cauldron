using System;

namespace Couldron
{
    /// <summary>
    /// Specifies which constructor should be used when instanciating an object
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public sealed class InjectionConstructorAttribute : Attribute
    {
    }
}