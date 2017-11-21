using System;
using System.Net;

namespace Cauldron.Core.Net
{
    /// <summary>
    /// Represents a exception that occures during Web operations
    /// </summary>
    public sealed class WebException : Exception
    {
        internal WebException(string message, HttpStatusCode code) : base(message)
        {
            this.StatusCode = code;
        }

        /// <summary>
        /// Gets the status code of the http request
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }
    }
}