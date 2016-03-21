using System;

namespace Couldron.Validation
{
    /// <summary>
    /// Specifies that the property change will not affect the model's change flag
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class SuppressIsChangedAttribute : Attribute
    {
    }
}