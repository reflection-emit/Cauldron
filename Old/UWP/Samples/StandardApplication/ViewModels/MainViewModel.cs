using Cauldron.XAML;
using Cauldron.XAML.ViewModels;
using StandardApplication.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StandardApplication.ViewModels
{
    [View(typeof(MainView))]
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            this.PopupMeCommand = new RelayCommand(this.PopupMeAction);
            this.IsLoading = false;
        }

        /// <summary>
        /// Gets the PopupMe command
        /// </summary>
        public ICommand PopupMeCommand { get; private set; }

        private async void PopupMeAction()
        {
            await this.Navigator.NavigateAsync<ThePopupViewModel, string>(x =>
             {
                 if (!string.IsNullOrEmpty(x))
                     this.MessageDialog.ShowOKAsync("Title", $"Content: {x}");

                 return Task.FromResult(0);
             });
        }
    }
}