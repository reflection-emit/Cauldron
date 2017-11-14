using System;

namespace Cauldron
{
    /// <summary>
    /// Represents a exception that occures while creating an instance using an interface
    /// </summary>
    public sealed class TypeIsInterfaceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TypeIsInterfaceException"/>
        /// </summary>
        /// <param name="message"></param>
        public TypeIsInterfaceException(string message) : base(message)
        {
        }
    }
}