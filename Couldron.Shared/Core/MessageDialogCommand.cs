using System;

#if NETFX_CORE
using Windows.UI.Popups;
#endif

namespace Couldron.Core
{
    public sealed class MessageDialogCommand
    {
        public string Label { get; set; }

        private Action command;

        public MessageDialogCommand(string label)
        {
            if (Factory.HasContract(typeof(ILocalizationSource)))
            {
                var localization = Factory.Create<Localization>();
                this.Label = localization[label];
            }
            else
            {
#if !NETFX_CORE
                // Try to use default windows strings from user32.dll
                string defaultWindowsString = Utils.GetStringFromModule(label);
                if (defaultWindowsString != null)
                    this.Label = defaultWindowsString;
                else
#endif
                this.Label = label;
            }
        }

        public MessageDialogCommand(string label, Action command)
        {
            this.Label = label;
            this.command = command;
        }

#if NETFX_CORE

        public UICommand ToUICommand()
        {
            if (this.command == null)
                return new UICommand(this.Label);
            else
                return new UICommand(this.Label, new UICommandInvokedHandler(x => this.command()));
        }

#else

        public void Invoke()
        {
            if (this.command != null)
                this.command();
        }

#endif
    }
}