using System;

namespace Cauldron.Consoles
{
    /// <summary>
    /// Represents an error that occured during the parsing. The parser was not able to identify the passed parameter.
    /// </summary>
    public sealed class UnknownParameterException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="UnknownParameterException"/>
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="parameter">The parameter that caused the error</param>
        public UnknownParameterException(string message, string parameter) : base(message) => this.Parameter = parameter;

        /// <summary>
        /// Gets the parameter that caused the error
        /// </summary>
        public string Parameter { get; private set; }
    }
}