using Couldron;
using Couldron.Aspects;
using Couldron.Core;
using Couldron.Messaging;
using Couldron.Validation;
using Couldron.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ViewModelCommunication.ViewModels
{
    public class MainViewModel : ValidatableViewModelBase
    {
        public MainViewModel()
        {
            this.Animals = new ObservableCollection<AnimalViewModel>();

            this.Animals.Add(Factory.Create<AnimalViewModel>("Anthropoides paradisea"));
            this.Animals.Add(Factory.Create<AnimalViewModel>("Branta canadensis"));
            this.Animals.Add(Factory.Create<AnimalViewModel>("Colaptes campestroides"));
            this.Animals.Add(Factory.Create<AnimalViewModel>("Thomson's gazelle"));

            MessageManager.Subscribe<CloseTabMessage>(this, OnCloseTab);

            this.LoadData();
        }

        public ObservableCollection<AnimalViewModel> Animals { get; private set; }

        [NotifyPropertyChanged]
        [IsMandatory("mandatory")]
        public AnimalViewModel SelectedAnimal { get; set; }

        [NotifyPropertyChanged]
        public BitmapImage UserImage { get; set; }

        [NotifyPropertyChanged]
        public string Username { get; set; }

        public void OnCloseTab(CloseTabMessage message)
        {
            this.Animals.Remove(message.Sender as AnimalViewModel);
        }

        private async void LoadData()
        {
            this.Username = await UserInformation.Current.GetDisplayNameAsync();
            this.UserImage = UserInformation.Current.GetAccountPicture();
        }
    }
}