using Cauldron.Activator;
using Cauldron.Core;
using Cauldron.Localization;
using Cauldron.XAML.Resources;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

#if WINDOWS_UWP

using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

#else

using System.Windows.Media.Imaging;

#endif

namespace Cauldron.XAML.ViewModels
{
    [View(typeof(MessageDialogView))]
    internal class MessageDialogViewModel : ViewModelBase, IDialogViewModel
    {
        private HorizontalAlignment _horizontalAlignment;
        private BitmapImage _icon;
        private string _message;
        private string _title;

        private uint cancelCommandIndex;

        private uint defaultCommandIndex;

        [ComponentConstructor]
        public MessageDialogViewModel(string title, string message, int messageBoxImage /* The serializer can only serialize primitives */, uint defaultCommandIndex, uint cancelCommandIndex, CauldronUICommandCollection commands)
        {
            var locale = Locale.Current;

            this.defaultCommandIndex = defaultCommandIndex;
            this.cancelCommandIndex = cancelCommandIndex;

            this.Title = string.IsNullOrEmpty(title) ? ApplicationInfo.ApplicationName : locale[title];
            this.Message = locale[message];

            this.LoadIconAsync((MessageBoxImage)messageBoxImage);

            this.HorizontalAlignment = HorizontalAlignment.Left;

            this.Buttons = new ObservableCollection<MessageDialogCommandViewModel>();
            this.CancelCommand = new RelayCommand(this.CancelAction);
            this.EnterCommand = new RelayCommand(this.EnterAction);

            this.AddCommands(commands);
        }

        public ObservableCollection<MessageDialogCommandViewModel> Buttons { get; private set; }

        /// <summary>
        /// Gets the Cancel command
        /// </summary>
        public ICommand CancelCommand { get; private set; }

        /// <summary>
        /// Gets the Enter command
        /// </summary>
        public ICommand EnterCommand { get; private set; }

        public HorizontalAlignment HorizontalAlignment
        {
            get { return this._horizontalAlignment; }
            set
            {
                if (this._horizontalAlignment == value)
                    return;

                this._horizontalAlignment = value;
                this.RaisePropertyChanged();
            }
        }

        public BitmapImage Icon
        {
            get { return this._icon; }
            set
            {
                if (this._icon == value)
                    return;

                this._icon = value;
                this.RaisePropertyChanged();
            }
        }

        public string Message
        {
            get { return this._message; }
            set
            {
                if (this._message == value)
                    return;

                this._message = value;
                this.RaisePropertyChanged();
            }
        }

        public string Title
        {
            get { return this._title; }
            set
            {
                if (this._title == value)
                    return;

                this._title = value;
                this.RaisePropertyChanged();
            }
        }

        private async void AddCommands(CauldronUICommandCollection commands)
        {
            await this.Dispatcher.RunAsync(() =>
             {
                 foreach (var item in commands.Where(x => x != null))
                 {
                     this.Buttons.Add(new MessageDialogCommandViewModel
                     {
                         Text = item.Label,
                         ButtonCommand = new RelayCommand(() =>
                         {
                             item.Invoke();
                             Navigator.TryClose(this);
                         })
                     });
                 }
             });
        }

        private void CancelAction()
        {
            if (this.Buttons.Count > 0 && this.cancelCommandIndex < this.Buttons.Count)
                this.Buttons[(int)this.cancelCommandIndex].ButtonCommand.Execute(null);
        }

        private void EnterAction()
        {
            if (this.Buttons.Count > 0 && this.defaultCommandIndex < this.Buttons.Count)
                this.Buttons[(int)this.defaultCommandIndex].ButtonCommand.Execute(null);
        }

        private async void LoadIconAsync(MessageBoxImage messageBoxImage)
        {
            switch (messageBoxImage)
            {
                case MessageBoxImage.Error:
                    this.Icon = await ImageManager.Current.GetImageAsync("error_red_32x32.png");
                    break;

                case MessageBoxImage.Question:
                    this.Icon = await ImageManager.Current.GetImageAsync("Information(Help)_7833.png");
                    break;

                case MessageBoxImage.Exclamation:
                    this.Icon = await ImageManager.Current.GetImageAsync("Warning_yellow_7231_31x32.png");
                    break;

                case MessageBoxImage.Information:
                    this.Icon = await ImageManager.Current.GetImageAsync("Information_6227_32x.png");
                    break;
            }
        }
    }
}