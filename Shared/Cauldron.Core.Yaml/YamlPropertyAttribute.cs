using System;

namespace Cauldron.Core.Yaml
{
    /// <summary>
    /// Instructs the Newtonsoft.Json.JsonSerializer to always serialize the member with the specified name.
    /// </summary>
    public sealed class YamlPropertyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="YamlPropertyAttribute"/>
        /// </summary>
        /// <param name="name">Name of the property.</param>
        public YamlPropertyAttribute(string name) => this.Name = name;

        /// <summary>
        /// Gets the name of the property
        /// </summary>
        public string Name { get; private set; }
    }
}