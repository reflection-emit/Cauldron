namespace Cauldron
{
    /// <summary>
    /// Provides message data
    /// </summary>
    public class MessagingArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MessagingArgs"/>
        /// </summary>
        /// <param name="sender">The source of the message</param>
        public MessagingArgs(object sender)
        {
            this.Sender = sender;
        }

        /// <summary>
        /// Gets the message source
        /// </summary>
        public object Sender { get; private set; }
    }
}