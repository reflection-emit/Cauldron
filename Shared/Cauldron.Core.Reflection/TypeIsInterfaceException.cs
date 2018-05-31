using System;
using System.Runtime.Serialization;

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

        /// <summary>
        /// Initializes a new instance of <see cref="TypeIsInterfaceException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        public TypeIsInterfaceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}