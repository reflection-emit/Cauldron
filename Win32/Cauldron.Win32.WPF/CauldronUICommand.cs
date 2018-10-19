using Cauldron.Activator;
using Cauldron.Localization;
using System;

#if WINDOWS_UWP

using Windows.UI.Popups;

#endif

namespace Cauldron.XAML
{
    /// <summary>
    /// Represents a command in the MessageDialog and ContentDialog
    /// </summary>
    public sealed class CauldronUICommand : ICauldronUICommand
    {
        private Action command;

        /// <summary>
        /// Initializes a new instance of the <see cref="CauldronUICommand"/> class
        /// </summary>
        /// <param name="label">The label of the command button</param>
        public CauldronUICommand(string label)
        {
            if (Factory.HasContract(typeof(ILocalizationSource)))
                this.Label = Locale.Current[label];
            else
                this.Label = label;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CauldronUICommand"/> class
        /// </summary>
        /// <param name="label">The label of the command button</param>
        /// <param name="command">The action that will be invoke if the command is executed</param>
        public CauldronUICommand(string label, Action command) : this(label)
        {
            this.command = command;
        }

        /// <summary>
        /// Gets or sets the label of the command button
        /// </summary>
        public string Label { get; private set; }

        //#if WINDOWS_UWP

        //        /// <summary>
        //        /// Converts a <see cref="CauldronUICommand"/> to a <see cref="UICommand"/>
        //        /// </summary>
        //        /// <returns></returns>
        //        public UICommand ToUICommand()
        //        {
        //            if (this.command == null)
        //                return new UICommand(this.Label);
        //            else
        //                return new UICommand(this.Label, new UICommandInvokedHandler(x => this.command()));
        //        }

        //#else
        /// <summary>
        /// Executes the command
        /// </summary>
        public void Invoke()
        {
            if (this.command != null)
                this.command();
        }

        //#endif
    }
}