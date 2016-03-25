namespace Couldron.Messaging
{
    public class MessagingArgs
    {
        public MessagingArgs(object sender)
        {
            this.Sender = sender;
        }

        public object Sender { get; private set; }
    }
}