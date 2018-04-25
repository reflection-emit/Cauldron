using System;

namespace Cauldron.WindowsService
{
    /// <summary>
    /// Represents an exception that occures while interacting with process tokens.
    /// </summary>
    public sealed class AccessTokenException : Exception
    {
        internal AccessTokenException(string message) : base(message)
        {
        }
    }
}