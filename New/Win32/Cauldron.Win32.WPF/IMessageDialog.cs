using System;
using System.Threading.Tasks;

namespace Cauldron.XAML
{
    /// <summary>
    /// Represents a dialog.
    /// </summary>
    public interface IMessageDialog
    {
        /// <summary>
        /// Begins an asynchronous operation showing a dialog.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="command1">A command that appear in the command bar of the message dialog. This command makes the dialog actionable.</param>
        /// <param name="command2">A command that appear in the command bar of the message dialog. This command makes the dialog actionable.</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        Task ShowAsync(string title, string content, CauldronUICommand command1, CauldronUICommand command2);

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
        Task ShowAsync(string title, string content, uint defaultCommandIndex, uint cancelCommandIndex, MessageBoxImage messageBoxImage, CauldronUICommandCollection commands);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="messageBoxImage">The icon to show on the dialog</param>
        /// <param name="command1">A command that appear in the command bar of the message dialog. This command makes the dialog actionable.</param>
        /// <param name="command2">A command that appear in the command bar of the message dialog. This command makes the dialog actionable.</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        Task ShowAsync(string title, string content, MessageBoxImage messageBoxImage, CauldronUICommand command1, CauldronUICommand command2);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="defaultCommandIndex">The index of the command you want to use as the default. This is the command that fires by default when users press the ENTER key.</param>
        /// <param name="cancelCommandIndex">The index of the command you want to use as the cancel command. This is the command that fires when users press the ESC key.</param>
        /// <param name="commands">An array of commands that appear in the command bar of the message dialog. These commands makes the dialog actionable.</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        Task ShowAsync(string title, string content, uint defaultCommandIndex, uint cancelCommandIndex, CauldronUICommandCollection commands);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK and a copy button
        /// </summary>
        /// <param name="e">The exception to show on the dialog</param>
        /// <param name="format">Additional information to be displayed to the user. The exception is added in {0}</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        Task ShowException(Exception e, string format);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK and a copy button
        /// </summary>
        /// <param name="e">The exception to show on the dialog</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        Task ShowException(Exception e);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        Task ShowOKAsync(string title, string content);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="command">The action associated with the OK Command</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        Task ShowOKAsync(string title, string content, Action command);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="messageBoxImage">The icon to show on the dialog</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        Task ShowOKAsync(string title, string content, MessageBoxImage messageBoxImage);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK button.
        /// </summary>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        Task ShowOKAsync(string content);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK and cancel button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="commandOK">The action associated with the OK Command</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        Task ShowOKCancelAsync(string title, string content, Action commandOK);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK and cancel button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="commandOK">The action associated with the OK Command</param>
        /// <param name="commandCancel">The action associated with the cancel Command</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        Task ShowOKCancelAsync(string title, string content, Action commandOK, Action commandCancel);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with a Yes and No button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <returns>Returns true if the user has selected Yes, otherwise false</returns>
        Task<bool> ShowYesNoAsync(string title, string content);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with a Yes and No button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="commandYes">The action associated with the Yes Command</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        Task ShowYesNoAsync(string title, string content, Action commandYes);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with a Yes and No button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="commandYes">The action associated with the Yes Command</param>
        /// <param name="commandNo">The action associated with the No Command</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        Task ShowYesNoAsync(string title, string content, Action commandYes, Action commandNo);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with a Yes, No and Cancel button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="commandYes">The action associated with the Yes Command</param>
        /// <param name="commandNo">The action associated with the No Command</param>
        /// <param name="commandCancel">The action associated with the Cancel Command</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        Task ShowYesNoCancelAsync(string title, string content, Action commandYes, Action commandNo, Action commandCancel);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with a Yes, No and Cancel button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="commandYes">The action associated with the Yes Command</param>
        /// <param name="commandNo">The action associated with the No Command</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        Task ShowYesNoCancelAsync(string title, string content, Action commandYes, Action commandNo);
    }
}