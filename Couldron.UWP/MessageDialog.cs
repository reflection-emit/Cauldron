using Couldron.Core;
using System;
using System.Threading.Tasks;

namespace Couldron
{
    /// <summary>
    /// Represents a dialog. The dialog has a command bar that can support up to three commands.
    /// </summary>
    public static partial class MessageDialog
    {
        /// <summary>
        /// Begins an asynchronous operation showing a dialog.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="defaultCommandIndex">The index of the command you want to use as the default. This is the command that fires by default when users press the ENTER key.</param>
        /// <param name="cancelCommandIndex">The index of the command you want to use as the cancel command. This is the command that fires when users press the ESC key.</param>
        /// <param name="commands">An array of commands that appear in the command bar of the message dialog. These commands makes the dialog actionable.</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public static async Task ShowAsync(string title, string content, uint defaultCommandIndex, uint cancelCommandIndex, MessageDialogCommandList commands)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(content, title);

            foreach (var command in commands)
                dialog.Commands.Add(command.ToUICommand());

            dialog.CancelCommandIndex = cancelCommandIndex;
            dialog.DefaultCommandIndex = defaultCommandIndex;

            await dialog.ShowAsync();
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="e">The exception to show on the dialog</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public static async Task ShowOKAsync(string title, Exception e)
        {
            await ShowAsync(title, e.InnerException == null ? e.Message : e.Message + "\r\n" + e.InnerException.Message, 0, 0, new MessageDialogCommandList()
                    {
                        new MessageDialogCommand("OK")
                    });
        }
    }
}