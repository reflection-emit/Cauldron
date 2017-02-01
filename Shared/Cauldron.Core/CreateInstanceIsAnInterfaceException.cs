using System;

namespace Cauldron.Core
{
    /// <summary>
    /// Represents a exception that occures while creating an instance using an interface
    /// </summary>
    public sealed class CreateInstanceIsAnInterfaceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CreateInstanceIsAnInterfaceException"/>
        /// </summary>
        /// <param name="message"></param>
        public CreateInstanceIsAnInterfaceException(string message) : base(message)
        {
        }
    }
}