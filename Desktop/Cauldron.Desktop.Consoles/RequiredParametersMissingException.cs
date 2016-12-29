using System;

namespace Cauldron.Consoles
{
    public sealed class RequiredParametersMissingException : Exception
    {
        internal RequiredParametersMissingException(string message, string[] missingRequiredParameters) : base(message)
        {
            this.MissingRequiredParameters = missingRequiredParameters;
        }

        public string[] MissingRequiredParameters { get; private set; }
    }
}