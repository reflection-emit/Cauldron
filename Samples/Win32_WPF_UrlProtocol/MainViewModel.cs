using Cauldron.XAML;
using Cauldron.XAML.ViewModels;
using System.Windows.Input;

namespace Win32_WPF_UrlProtocol
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            this.RegisterCommand = new RelayCommand(this.RegisterAction);
        }

        /// <summary>
        /// Gets the Register command
        /// </summary>
        public ICommand RegisterCommand { get; private set; }

        private void RegisterAction() => App.RegisterUrlProtocols();
    }
}