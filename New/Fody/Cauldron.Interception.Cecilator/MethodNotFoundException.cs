using System;

namespace Cauldron.Interception.Cecilator
{
    public sealed class MethodNotFoundException : Exception
    {
        internal MethodNotFoundException(string message) : base(message)
        {
        }
    }
}