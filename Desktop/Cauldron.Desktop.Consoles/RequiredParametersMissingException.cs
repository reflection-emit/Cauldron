using System;

namespace Cauldron.Consoles
{
    /// <summary>
    /// Represents a error that occured during the parsing. A required parameter has no assigned value.
    /// </summary>
    public sealed class RequiredParametersMissingException : Exception
    {
        internal RequiredParametersMissingException(string message, string[] missingRequiredParameters) : base(message)
        {
            this.MissingRequiredParameters = missingRequiredParameters;
        }

        /// <summary>
        /// Gets the required parameters without value
        /// </summary>
        public string[] MissingRequiredParameters { get; private set; }
    }
}