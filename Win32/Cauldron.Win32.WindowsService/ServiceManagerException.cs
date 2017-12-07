using System;

namespace Cauldron.WindowsService
{
    /// <summary>
    /// Represents an exception that occures while interacting with the service manager.
    /// </summary>
    public sealed class ServiceManagerException : Exception
    {
        internal ServiceManagerException(string message) : base(message)
        {
        }
    }
}