using Cauldron.Core;
using Cauldron.XAML;
using Cauldron.XAML.ViewModels;
using StandardApplication.Views;
using System.Windows.Input;

namespace StandardApplication.ViewModels
{
    [View(typeof(ThePopupView))]
    public class ThePopupViewModel : ViewModelBase, IDialogViewModel<string>
    {
        public ThePopupViewModel()
        {
            this.CloseCommand = new RelayCommand(this.CloseAction);
        }

        public string AText { get; set; }

        /// <summary>
        /// Gets the Close command
        /// </summary>
        public ICommand CloseCommand { get; private set; }

        public string Result { get; set; }

        public string Title { get { return ApplicationInfo.ApplicationName; } }

        private void CloseAction()
        {
            this.Result = this.AText;
            this.TryClose();
        }
    }
}