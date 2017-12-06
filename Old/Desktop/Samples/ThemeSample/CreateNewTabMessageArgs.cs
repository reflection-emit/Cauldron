using Cauldron.Core;
using System;

namespace ThemeSample
{
    internal class CreateNewTabMessageArgs : MessagingArgs
    {
        public CreateNewTabMessageArgs(object sender, Type viewModelType) : base(sender)
        {
            this.ViewModelType = viewModelType;
        }

        public Type ViewModelType { get; private set; }
    }
}