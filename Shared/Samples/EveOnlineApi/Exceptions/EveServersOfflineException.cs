using System;

namespace EveOnlineApi.Exceptions
{
    public sealed class EveServersOfflineException : Exception
    {
        public EveServersOfflineException(string message) : base(message)
        {
        }
    }
}