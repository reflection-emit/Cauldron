using Cauldron.Interception.Cecilator;
using System;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class HelperTypeFieldAttribute : Attribute
    {
        public HelperTypeFieldAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }

        public Field GetField(BuilderType builderType) => builderType.GetField(this.Name, true).Import();
    }
}