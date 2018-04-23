using System;

namespace Cauldron.Interception.Fody
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class DisplayAttribute : Attribute
    {
        public DisplayAttribute(string name) => this.Name = name;

        public string Name { get; }
    }
}