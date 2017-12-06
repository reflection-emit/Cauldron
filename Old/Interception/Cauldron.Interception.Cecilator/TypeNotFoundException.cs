using System;

namespace Cauldron.Interception.Cecilator
{
    public sealed class TypeNotFoundException : Exception
    {
        internal TypeNotFoundException(string message) : base(message)
        {
        }
    }
}