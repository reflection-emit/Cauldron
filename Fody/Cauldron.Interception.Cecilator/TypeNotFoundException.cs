using System;

namespace Cauldron.Interception.Cecilator
{
    public sealed class TypeNotFoundException : Exception
    {
        public TypeNotFoundException(string message) : base(message)
        {
        }
    }
}