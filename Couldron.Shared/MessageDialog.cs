using Couldron.Core;
using System;
using System.Threading.Tasks;

namespace Couldron
{
    public static partial class MessageDialog
    {
        /// <summary>
        /// Begins an asynchronous operation showing a dialog.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="command1">A command that appear in the command bar of the message dialog. This command makes the dialog actionable.</param>
        /// <param name="command2">A command that appear in the command bar of the message dialog. This command makes the dialog actionable.</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public static async Task ShowAsync(string title, string content, MessageDialogCommand command1, MessageDialogCommand command2)
        {
            await ShowAsync(title, content, 0, 1, new MessageDialogCommandList()
                    {
                        command1,
                        command2
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK button.
        /// </summary>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public static async Task ShowOKAsync(string content)
        {
            await ShowAsync(string.Empty, content, 0, 0, new MessageDialogCommandList()
                    {
                        new MessageDialogCommand("OK")
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public static async Task ShowOKAsync(string title, string content)
        {
            await ShowAsync(title, content, 0, 0, new MessageDialogCommandList()
                    {
                        new MessageDialogCommand("OK")
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="command">The action associated with the OK Command</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public static async Task ShowOKAsync(string title, string content, Action command)
        {
            await ShowAsync(title, content, 0, 0, new MessageDialogCommandList()
                    {
                        new MessageDialogCommand("OK", command)
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK and cancel button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="commandOK">The action associated with the OK Command</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public static async Task ShowOKCancelAsync(string title, string content, Action commandOK)
        {
            await ShowAsync(title, content, 0, 1, new MessageDialogCommandList()
                    {
                        new MessageDialogCommand("OK", commandOK),
                        new MessageDialogCommand("Cancel")
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK and cancel button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="commandOK">The action associated with the OK Command</param>
        /// <param name="commandCancel">The action associated with the cancel Command</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public static async Task ShowOKCancelAsync(string title, string content, Action commandOK, Action commandCancel)
        {
            await ShowAsync(title, content, 0, 1, new MessageDialogCommandList()
                    {
                        new MessageDialogCommand("OK", commandOK),
                        new MessageDialogCommand("Cancel", commandCancel)
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with a Yes and No button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <returns>Returns true if the user has selected Yes, otherwise false</returns>
        public static async Task<bool> ShowYesNoAsync(string title, string content)
        {
            bool result = false;

            await ShowAsync(title, content, 0, 1, new MessageDialogCommandList()
                    {
                        new MessageDialogCommand("Yes", ()=> result = true),
                        new MessageDialogCommand("No", ()=> result = false)
                    });

            return result;
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with a Yes and No button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="commandYes">The action associated with the Yes Command</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public static async Task ShowYesNoAsync(string title, string content, Action commandYes)
        {
            await ShowAsync(title, content, 0, 1, new MessageDialogCommandList()
                    {
                        new MessageDialogCommand("Yes", commandYes),
                        new MessageDialogCommand("No")
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with a Yes and No button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="commandYes">The action associated with the Yes Command</param>
        /// <param name="commandNo">The action associated with the No Command</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public static async Task ShowYesNoAsync(string title, string content, Action commandYes, Action commandNo)
        {
            await ShowAsync(title, content, 0, 1, new MessageDialogCommandList()
                    {
                        new MessageDialogCommand("Yes", commandYes),
                        new MessageDialogCommand("No", commandNo)
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with a Yes, No and Cancel button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="commandYes">The action associated with the Yes Command</param>
        /// <param name="commandNo">The action associated with the No Command</param>
        /// <param name="commandCancel">The action associated with the Cancel Command</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public static async Task ShowYesNoCancelAsync(string title, string content, Action commandYes, Action commandNo, Action commandCancel)
        {
            await ShowAsync(title, content, 0, 2, new MessageDialogCommandList()
                    {
                        new MessageDialogCommand("Yes", commandYes),
                        new MessageDialogCommand("No", commandNo),
                        new MessageDialogCommand("Cancel", commandCancel)
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with a Yes, No and Cancel button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="commandYes">The action associated with the Yes Command</param>
        /// <param name="commandNo">The action associated with the No Command</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public static async Task ShowYesNoCancelAsync(string title, string content, Action commandYes, Action commandNo)
        {
            await ShowAsync(title, content, 0, 2, new MessageDialogCommandList()
                    {
                        new MessageDialogCommand("Yes", commandYes),
                        new MessageDialogCommand("No", commandNo),
                        new MessageDialogCommand("Cancel")
                    });
        }
    }
}