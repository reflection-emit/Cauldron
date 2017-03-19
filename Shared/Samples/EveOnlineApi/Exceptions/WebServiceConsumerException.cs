using System;

namespace EveOnlineApi.Exceptions
{
    public sealed class WebServiceConsumerException : Exception
    {
        public WebServiceConsumerException(string message) : base(message)
        {
        }

        public WebServiceConsumerException(string message, Exception e) : base(message, e)
        {
        }

        public string Response { get; set; }

        public string Url { get; set; }
    }
}