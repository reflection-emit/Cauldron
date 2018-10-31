using System;

namespace Cauldron
{
    internal sealed class MessageObject
    {
        public Action<MessagingArgs> Handler { get; set; }
        public Type MessageType { get; set; }
        public object Subscriber { get; set; }
    }
}