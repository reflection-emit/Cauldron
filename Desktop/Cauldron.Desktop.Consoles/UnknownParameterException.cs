using System;

namespace Cauldron.Consoles
{
    public sealed class UnknownParameterException : Exception
    {
        internal UnknownParameterException(string message, string parameter) : base(message)
        {
            this.Parameter = parameter;
        }

        public string Parameter { get; private set; }
    }
}