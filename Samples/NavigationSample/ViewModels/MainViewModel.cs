using Couldron;
using Couldron.Aspects;
using Couldron.ViewModels;
using NavigationSample.View;
using System.Windows.Input;

namespace NavigationSample.ViewModels
{
    [View(typeof(MainView))]
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            this.OpenPopupWindowCommand = new RelayCommand(this.OpenPopupAction);

            this.Title = "Main View Model";
        }

        public ICommand OpenPopupWindowCommand { get; private set; }

        [NotifyPropertyChanged]
        public string Title { get; set; }

        private void OpenPopupAction()
        {
            Navigator.Navigate<OtherViewModel, string>(x => this.Title = x, this.Title);
        }
    }
}