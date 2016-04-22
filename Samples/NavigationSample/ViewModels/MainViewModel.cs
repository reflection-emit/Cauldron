using Cauldron;
using Cauldron.ViewModels;
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
            this.NewInstanceCommand = new RelayCommand(this.NewInstanceAction);

            this.Title = "Main View Model";
        }

        public ICommand NewInstanceCommand { get; private set; }
        public ICommand OpenPopupWindowCommand { get; private set; }

        public string Title { get; set; }

        private void NewInstanceAction()
        {
            Navigator.Navigate<MainViewModel>();
        }

        private void OpenPopupAction()
        {
            Navigator.Navigate<OtherViewModel, string>(x => this.Title = x, this.Title);
        }
    }
}