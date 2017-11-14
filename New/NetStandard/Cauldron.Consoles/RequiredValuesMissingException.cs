using System;

namespace Cauldron.Consoles
{
    /// <summary>
    /// Represents a error that occured during the parsing. A parameter with non optional value has no assigned value.
    /// </summary>
    public sealed class RequiredValuesMissingException : Exception
    {
        internal RequiredValuesMissingException(string message, string[] missingRequiredParameters) : base(message)
        {
            this.MissingRequiredParameters = missingRequiredParameters;
        }

        /// <summary>
        /// Gets the parameters with non optional value without value
        /// </summary>
        public string[] MissingRequiredParameters { get; private set; }
    }
}