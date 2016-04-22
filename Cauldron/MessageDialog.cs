using Cauldron.Core;
using Cauldron.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Cauldron
{
    /// <summary>
    /// Represents a dialog.
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
            await ShowAsync(title, content, defaultCommandIndex, cancelCommandIndex, MessageBoxImage.None, commands);
        }

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
        public static async Task ShowAsync(string title, string content, uint defaultCommandIndex, uint cancelCommandIndex, MessageBoxImage messageBoxImage, MessageDialogCommandList commands)
        {
            BitmapImage icon = null;

            switch (messageBoxImage)
            {
                case MessageBoxImage.Error:
                    icon = EmbeddedImageManager.GetImage("error_red_32x32.png");
                    break;

                case MessageBoxImage.Question:
                    icon = EmbeddedImageManager.GetImage("Information(Help)_7833.png");
                    break;

                case MessageBoxImage.Exclamation:
                    icon = EmbeddedImageManager.GetImage("Warning_yellow_7231_31x32.png");
                    break;

                case MessageBoxImage.Information:
                    icon = EmbeddedImageManager.GetImage("Information_6227_32x.png");
                    break;
            }

            await Task.Delay(0);

            if (icon == null)
                Navigator.Navigate<MessageDialogViewModel>(title, content, defaultCommandIndex, cancelCommandIndex, commands);
            else
                Navigator.Navigate<MessageDialogViewModel>(title, content, icon, defaultCommandIndex, cancelCommandIndex, commands);
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="messageBoxImage">The icon to show on the dialog</param>
        /// <param name="command1">A command that appear in the command bar of the message dialog. This command makes the dialog actionable.</param>
        /// <param name="command2">A command that appear in the command bar of the message dialog. This command makes the dialog actionable.</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public static async Task ShowAsync(string title, string content, MessageBoxImage messageBoxImage, MessageDialogCommand command1, MessageDialogCommand command2)
        {
            await ShowAsync(title, content, 0, 1, messageBoxImage, new MessageDialogCommandList()
                    {
                        command1,
                        command2
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="messageBoxImage">The icon to show on the dialog</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public static async Task ShowOKAsync(string title, string content, MessageBoxImage messageBoxImage)
        {
            await ShowAsync(title, content, 0, 0, messageBoxImage, new MessageDialogCommandList()
                    {
                        new MessageDialogCommand("OK")
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="e">The exception to show on the dialog</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public static async Task ShowOKAsync(string title, Exception e)
        {
            await ShowAsync(title, e.InnerException == null ? e.Message : e.Message + "\r\n" + e.InnerException.Message, 0, 0, MessageBoxImage.Error, new MessageDialogCommandList()
                    {
                        new MessageDialogCommand("OK")
                    });
        }
    }
}