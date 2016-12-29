using System;

namespace Cauldron.Consoles
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class OptionAttribute : Attribute
    {
        public OptionAttribute(string description, bool isRequired, params string[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                throw new ArgumentException("parameters cannot be null or empty");

            this.Parameters = parameters;
            this.IsRequired = isRequired;
            this.Description = description;
        }

        public OptionAttribute(string description, params string[] parameters) : this(description, false, parameters)
        {
        }

        public string Description { get; private set; }
        public bool IsRequired { get; private set; }
        public string[] Parameters { get; private set; }
    }
}