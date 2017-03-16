using System;

namespace Cauldron.Interception.Cecilator
{
    public sealed class FieldInUseException : Exception
    {
        internal FieldInUseException(string message) : base(message)
        {
        }
    }
}