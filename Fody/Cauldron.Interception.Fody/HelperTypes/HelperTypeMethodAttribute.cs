using Cauldron.Interception.Cecilator;
using System;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class HelperTypeMethodAttribute : Attribute
    {
        public HelperTypeMethodAttribute(string name, int parameterCount)
        {
            this.Name = name;
            this.ParameterCount = parameterCount;
        }

        public HelperTypeMethodAttribute(string name) : this(name, 0)
        {
        }

        public HelperTypeMethodAttribute(string name, params string[] parameterTypes)
        {
            this.Name = name;
            this.ParameterTypes = parameterTypes;
        }

        public string Name { get; private set; }
        public int ParameterCount { get; private set; }
        public string[] ParameterTypes { get; private set; }

        public Method GetMethod(BuilderType builderType)
        {
            if (this.ParameterTypes == null || this.ParameterTypes.Length == 0)
                return builderType.GetMethod(this.Name, this.ParameterCount, true);

            return builderType.GetMethod(this.Name, true, this.ParameterTypes);
        }
    }
}