using Couldron.Messaging;

namespace ViewModelCommunication
{
    public class CloseTabMessage : MessagingArgs
    {
        public CloseTabMessage(object sender) : base(sender)
        {
        }
    }
}