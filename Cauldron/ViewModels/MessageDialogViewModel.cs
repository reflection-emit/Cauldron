using Cauldron.Core;
using Cauldron.Resources;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Cauldron.ViewModels
{
    [View(typeof(MessageDialogView))]
    [Navigating(nameof(OnNavigate), nameof(OnNavigateWithoutIcon))]
    internal sealed class MessageDialogViewModel : ViewModelBase
    {
        private HorizontalAlignment _horizontalAlignment;
        private BitmapImage _icon;
        private string _message;
        private string _title;

        private uint cancelCommandIndex;

        private uint defaultCommandIndex;

        public MessageDialogViewModel()
        {
            this.Buttons = new ObservableCollection<MessageDialogCommandViewModel>();
            this.CancelCommand = new RelayCommand(this.CancelAction);
            this.EnterCommand = new RelayCommand(this.EnterAction);
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
                this.OnPropertyChanged();
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
                this.OnPropertyChanged();
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
                this.OnPropertyChanged();
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
                this.OnPropertyChanged();
            }
        }

        private void AddCommands(MessageDialogCommandList commands)
        {
            foreach (var item in commands)
            {
                this.Buttons.Add(new MessageDialogCommandViewModel
                {
                    Text = item.Label,
                    ButtonCommand = new RelayCommand(() =>
                    {
                        item.Invoke();
                        Navigator.CloseWindowOf(this);
                    })
                });
            }
        }

        private void CancelAction()
        {
            if (this.Buttons.Count > 0)
                this.Buttons[(int)this.cancelCommandIndex].ButtonCommand.Execute(null);
        }

        private void EnterAction()
        {
            if (this.Buttons.Count > 0)
                this.Buttons[(int)this.defaultCommandIndex].ButtonCommand.Execute(null);
        }

        private void OnNavigate(string title, string message, BitmapImage icon, uint defaultCommandIndex, uint cancelCommandIndex, MessageDialogCommandList commands)
        {
            this.defaultCommandIndex = defaultCommandIndex;
            this.cancelCommandIndex = cancelCommandIndex;

            this.Title = title;
            this.Message = message;
            this.Icon = icon;

            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.AddCommands(commands);
        }

        private void OnNavigateWithoutIcon(string title, string message, uint defaultCommandIndex, uint cancelCommandIndex, MessageDialogCommandList commands)
        {
            this.defaultCommandIndex = defaultCommandIndex;
            this.cancelCommandIndex = cancelCommandIndex;

            this.Title = title;
            this.Message = message;

            this.HorizontalAlignment = this.Message.Contains("\n") ? HorizontalAlignment.Left : HorizontalAlignment.Center;
            this.AddCommands(commands);
        }
    }
}