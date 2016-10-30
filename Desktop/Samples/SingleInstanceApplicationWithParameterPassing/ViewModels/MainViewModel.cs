using Cauldron.Core;
using Cauldron.XAML;
using Cauldron.XAML.ViewModels;
using SingleInstanceApplicationWithParameterPassing.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace SingleInstanceApplicationWithParameterPassing.ViewModels
{
    [View(typeof(MainView))]
    public sealed class MainViewModel : ViewModelBase
    {
        public MainViewModel(string[] arguments)
        {
            foreach (var argument in arguments)
                this.Arguments.Add(argument);

            this.OpenConsoleCommand = new RelayCommand(this.OpenConsoleAction);
            MessageManager.Subscribe<ParameterMessagingArgs>(this, this.ArgumentActivation);
        }

        public ObservableCollection<string> Arguments { get; private set; } = new ObservableCollection<string>();

        /// <summary>
        /// Gets the OpenConsole command
        /// </summary>
        public ICommand OpenConsoleCommand { get; private set; }

        private void ArgumentActivation(ParameterMessagingArgs args)
        {
            if (args.Sender.GetType() == typeof(App))
            {
                foreach (var argument in args.Arguments)
                    this.Arguments.Add(argument);
            }
        }

        private void OpenConsoleAction()
        {
            var processInfo = new ProcessStartInfo("cmd");
            processInfo.WorkingDirectory = ApplicationInfo.ApplicationPath;

            Process.Start(processInfo);
        }
    }
}