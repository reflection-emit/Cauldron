using System;

namespace Cauldron
{
    /// <summary>
    /// Specifies additional name for an <see cref="Enum"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class DisplayNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayNameAttribute"/> class
        /// </summary>
        /// <param name="name"></param>
        public DisplayNameAttribute(string name)
        {
            this.DisplayName = name;
        }

        /// <summary>
        /// Gets the display name of the enum value
        /// </summary>
        public string DisplayName { get; private set; }
    }
}