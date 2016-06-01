using Cauldron.Core;
using System.Threading.Tasks;

namespace Cauldron
{
    /// <summary>
    /// Represents a dialog.
    /// </summary>
    public partial interface IMessageDialog
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
        Task ShowAsync(string title, string content, uint defaultCommandIndex, uint cancelCommandIndex, MessageDialogCommandList commands);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="defaultCommandIndex">The index of the command you want to use as the default. This is the command that fires by default when users press the ENTER key.</param>
        /// <param name="cancelCommandIndex">The index of the command you want to use as the cancel command. This is the command that fires when users press the ESC key.</param>
        /// <param name="messageBoxImage">The icon to show on the dialog</param>
        /// <param name="commands">An array of commands that appear in the command bar of the message dialog. These commands makes the dialog actionable.</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        Task ShowAsync(string title, string content, uint defaultCommandIndex, uint cancelCommandIndex, MessageBoxImage messageBoxImage, MessageDialogCommandList commands);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="messageBoxImage">The icon to show on the dialog</param>
        /// <param name="command1">A command that appear in the command bar of the message dialog. This command makes the dialog actionable.</param>
        /// <param name="command2">A command that appear in the command bar of the message dialog. This command makes the dialog actionable.</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        Task ShowAsync(string title, string content, MessageBoxImage messageBoxImage, MessageDialogCommand command1, MessageDialogCommand command2);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="messageBoxImage">The icon to show on the dialog</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        Task ShowOKAsync(string title, string content, MessageBoxImage messageBoxImage);
    }
}