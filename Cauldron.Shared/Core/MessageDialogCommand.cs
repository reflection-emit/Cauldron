using System;

#if NETFX_CORE
using Windows.UI.Popups;
#endif

namespace Cauldron.Core
{
    /// <summary>
    /// Represents a command in the <see cref="MessageDialog"/>
    /// </summary>
    public sealed class MessageDialogCommand
    {
        /// <summary>
        /// Gets or sets the label of the command button
        /// </summary>
        public string Label { get; set; }

        private Action command;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDialogCommand"/> class
        /// </summary>
        /// <param name="label">The label of the command button</param>
        public MessageDialogCommand(string label)
        {
            if (Factory.HasContract(typeof(ILocalizationSource)))
            {
                var localization = Factory.Create<Localization>();
                var result = localization[label];

                if (result == null)
                {
#if !NETFX_CORE
                    var defaultWindows = Utils.Current.GetStringFromModule(label);
                    if (defaultWindows != null)
                        this.Label = defaultWindows;
                    else
#endif
                        this.Label = label + " *missing*";
                }
                else
                    this.Label = result;
            }
            else
            {
#if !NETFX_CORE
                // Try to use default windows strings from user32.dll
                string defaultWindowsString = Utils.Current.GetStringFromModule(label);
                if (defaultWindowsString != null)
                    this.Label = defaultWindowsString;
                else
#endif
                    this.Label = label;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDialogCommand"/> class
        /// </summary>
        /// <param name="label">The label of the command button</param>
        /// <param name="command">The action that will be invoke if the command is executed</param>
        public MessageDialogCommand(string label, Action command) : this(label)
        {
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

        /// <summary>
        /// Executes the command
        /// </summary>
        public void Invoke()
        {
            if (this.command != null)
                this.command();
        }

#endif
    }
}