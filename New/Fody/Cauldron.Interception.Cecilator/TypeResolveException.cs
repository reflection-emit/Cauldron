using System;

namespace Cauldron.Interception.Cecilator
{
    public sealed class TypeResolveException : Exception
    {
        public TypeResolveException(string message) : base(message)
        {
        }

        internal TypeResolveException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}