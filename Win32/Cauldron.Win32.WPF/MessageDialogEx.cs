using Cauldron.Activator;
using Cauldron;
using Cauldron.Threading;
using Cauldron.Localization;
using Cauldron.XAML.Navigation;
using Cauldron.XAML.ViewModels;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

#if WINDOWS_UWP

using Windows.ApplicationModel.DataTransfer;

#endif

namespace Cauldron.XAML
{
    /// <summary>
    /// Represents a dialog.
    /// </summary>
    [Component(typeof(IMessageDialog), FactoryCreationPolicy.Singleton)]
    public sealed class MessageDialogEx : IMessageDialog
    {
        private IDispatcher _dispatcher;
        private INavigator navigator;

        /// <exclude/>
        [ComponentConstructor]
        public MessageDialogEx()
        {
            this.navigator = Factory.Create<INavigator>();
        }

        /// <summary>
        /// Gets the <see cref="Dispatcher"/> this <see cref="IDispatcher "/> is associated with.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IDispatcher Dispatcher
        {
            get
            {
                if (this._dispatcher == null)
                    this._dispatcher = Factory.Create<IDispatcher>();

                return this._dispatcher;
            }
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="command1">
        /// A command that appear in the command bar of the message dialog. This command makes the
        /// dialog actionable.
        /// </param>
        /// <param name="command2">
        /// A command that appear in the command bar of the message dialog. This command makes the
        /// dialog actionable.
        /// </param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public async Task ShowAsync(string title, string content, CauldronUICommand command1, CauldronUICommand command2)
        {
            await this.ShowAsync(title, content, 0, 1, new CauldronUICommandCollection()
                    {
                        command1,
                        command2
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="defaultCommandIndex">
        /// The index of the command you want to use as the default. This is the command that fires
        /// by default when users press the ENTER key.
        /// </param>
        /// <param name="cancelCommandIndex">
        /// The index of the command you want to use as the cancel command. This is the command that
        /// fires when users press the ESC key.
        /// </param>
        /// <param name="commands">
        /// An array of commands that appear in the command bar of the message dialog. These commands
        /// makes the dialog actionable.
        /// </param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public async Task ShowAsync(string title, string content, uint defaultCommandIndex, uint cancelCommandIndex, CauldronUICommandCollection commands) =>
            await this.ShowAsync(title, content, defaultCommandIndex, cancelCommandIndex, MessageBoxImage.None, commands);

        /// <summary>
        /// Begins an asynchronous operation showing a dialog.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="messageBoxImage">The icon to show on the dialog</param>
        /// <param name="command1">
        /// A command that appear in the command bar of the message dialog. This command makes the
        /// dialog actionable.
        /// </param>
        /// <param name="command2">
        /// A command that appear in the command bar of the message dialog. This command makes the
        /// dialog actionable.
        /// </param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public async Task ShowAsync(string title, string content, MessageBoxImage messageBoxImage, CauldronUICommand command1, CauldronUICommand command2)
        {
            await this.ShowAsync(title, content, 0, 1, messageBoxImage, new CauldronUICommandCollection()
                    {
                        command1,
                        command2
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="defaultCommandIndex">
        /// The index of the command you want to use as the default. This is the command that fires
        /// by default when users press the ENTER key.
        /// </param>
        /// <param name="cancelCommandIndex">
        /// The index of the command you want to use as the cancel command. This is the command that
        /// fires when users press the ESC key.
        /// </param>
        /// <param name="messageBoxImage">The icon to show on the dialog</param>
        /// <param name="commands">
        /// An array of commands that appear in the command bar of the message dialog. These commands
        /// makes the dialog actionable.
        /// </param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public Task ShowAsync(string title, string content, uint defaultCommandIndex, uint cancelCommandIndex, MessageBoxImage messageBoxImage, CauldronUICommandCollection commands)
        {
            return this.Dispatcher.RunAsync(async () => await this.navigator.NavigateAsync<MessageDialogViewModel>(new Func<Task>(() => Task.FromResult(0)), title, content, (int)messageBoxImage, defaultCommandIndex, cancelCommandIndex, commands));
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK and a copy button
        /// </summary>
        /// <param name="e">The exception to show on the dialog</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public async Task ShowExceptionAsync(Exception e) => await this.ShowExceptionAsync(e, "{0}");

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK and a copy button
        /// </summary>
        /// <param name="e">The exception to show on the dialog</param>
        /// <param name="format">
        /// Additional information to be displayed to the user. The exception is added in {0}
        /// </param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public async Task ShowExceptionAsync(Exception e, string format)
        {
            string title = "Error";
            string OK = "OK";
            string Copy = "Copy";

            await this.ShowAsync(Locale.Current[title],
               string.Format(Locale.GetCurrentCultureInfo(), format,
                e.InnerException == null ? e.Message : e.Message + "\r\n" + e.InnerException.Message),
                0, 0, MessageBoxImage.Error, new CauldronUICommandCollection()
                    {
                        new CauldronUICommand(OK),
#if WINDOWS_UWP
                    new CauldronUICommand(Copy, ()=> {
                        var dataPackage = new DataPackage();
                        dataPackage.SetText(e.StackTrace);
                        Clipboard.SetContent(dataPackage);
                    })
#else
                    new CauldronUICommand(Copy, ()=> System.Windows.Clipboard.SetText(e.StackTrace))
#endif
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK button.
        /// </summary>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public async Task ShowOKAsync(string content)
        {
            string OK = "OK";

            await this.ShowAsync(string.Empty, content, 0, 0, new CauldronUICommandCollection()
                    {
                        new CauldronUICommand(OK)
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public async Task ShowOKAsync(string title, string content)
        {
            string OK = "OK";
            await this.ShowAsync(title, content, 0, 0, new CauldronUICommandCollection()
                    {
                        new CauldronUICommand(OK)
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="command">The action associated with the OK Command</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public async Task ShowOKAsync(string title, string content, Action command)
        {
            string OK = "OK";
            await this.ShowAsync(title, content, 0, 0, new CauldronUICommandCollection()
                    {
                        new CauldronUICommand(OK, command)
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="messageBoxImage">The icon to show on the dialog</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public async Task ShowOKAsync(string title, string content, MessageBoxImage messageBoxImage)
        {
            string OK = "OK";
            await this.ShowAsync(title, content, 0, 0, messageBoxImage, new CauldronUICommandCollection()
                    {
                        new CauldronUICommand(OK)
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with an OK and cancel button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <param name="commandOK">The action associated with the OK Command</param>
        /// <returns>An object that represents the asynchronous operation.</returns>
        public async Task ShowOKCancelAsync(string title, string content, Action commandOK)
        {
            string OK = "OK";
            string Cancel = "Cancel";
            await this.ShowAsync(title, content, 0, 1, new CauldronUICommandCollection()
                    {
                        new CauldronUICommand(OK, commandOK),
                        new CauldronUICommand(Cancel)
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
        public async Task ShowOKCancelAsync(string title, string content, Action commandOK, Action commandCancel)
        {
            string OK = "OK";
            string Cancel = "Cancel";

            await this.ShowAsync(title, content, 0, 1, new CauldronUICommandCollection()
                    {
                        new CauldronUICommand(OK, commandOK),
                        new CauldronUICommand(Cancel, commandCancel)
                    });
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog with a Yes and No button.
        /// </summary>
        /// <param name="title">The title to display on the dialog, if any.</param>
        /// <param name="content">The message to be displayed to the user.</param>
        /// <returns>Returns true if the user has selected Yes, otherwise false</returns>
        public async Task<bool> ShowYesNoAsync(string title, string content)
        {
            string Yes = "Yes";
            string No = "No";
            bool result = false;

            await this.ShowAsync(title, content, 0, 1, new CauldronUICommandCollection()
                    {
                        new CauldronUICommand(Yes, ()=> result = true),
                        new CauldronUICommand(No, ()=> result = false)
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
        public async Task ShowYesNoAsync(string title, string content, Action commandYes)
        {
            string Yes = "Yes";
            string No = "No";
            await this.ShowAsync(title, content, 0, 1, new CauldronUICommandCollection()
                    {
                        new CauldronUICommand(Yes, commandYes),
                        new CauldronUICommand(No)
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
        public async Task ShowYesNoAsync(string title, string content, Action commandYes, Action commandNo)
        {
            string Yes = "Yes";
            string No = "No";
            await this.ShowAsync(title, content, 0, 1, new CauldronUICommandCollection()
                    {
                        new CauldronUICommand(Yes, commandYes),
                        new CauldronUICommand(No, commandNo)
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
        public async Task ShowYesNoCancelAsync(string title, string content, Action commandYes, Action commandNo, Action commandCancel)
        {
            string Yes = "Yes";
            string No = "No";
            string Cancel = "Cancel";
            await this.ShowAsync(title, content, 0, 2, new CauldronUICommandCollection()
                    {
                        new CauldronUICommand(Yes, commandYes),
                        new CauldronUICommand(No, commandNo),
                        new CauldronUICommand(Cancel, commandCancel)
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
        public async Task ShowYesNoCancelAsync(string title, string content, Action commandYes, Action commandNo)
        {
            string Yes = "Yes";
            string No = "No";
            string Cancel = "Cancel";
            await this.ShowAsync(title, content, 0, 2, new CauldronUICommandCollection()
                    {
                        new CauldronUICommand(Yes, commandYes),
                        new CauldronUICommand(No, commandNo),
                        new CauldronUICommand(Cancel)
                    });
        }
    }
}