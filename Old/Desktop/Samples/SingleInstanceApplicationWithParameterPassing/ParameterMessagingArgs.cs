using Cauldron.Core;

namespace SingleInstanceApplicationWithParameterPassing
{
    public class ParameterMessagingArgs : MessagingArgs
    {
        public ParameterMessagingArgs(object sender, string[] arguments) : base(sender)
        {
            this.Arguments = arguments;
        }

        public string[] Arguments { get; private set; }
    }
}