using Couldron;
using Couldron.Aspects;
using Couldron.Messaging;
using Couldron.Validation;
using Couldron.ViewModels;
using System.Collections.ObjectModel;

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
        }

        public ObservableCollection<AnimalViewModel> Animals { get; private set; }

        [NotifyPropertyChanged]
        [IsMandatory("mandatory")]
        public AnimalViewModel SelectedAnimal { get; set; }

        public void OnCloseTab(CloseTabMessage message)
        {
            this.Animals.Remove(message.Sender as AnimalViewModel);
        }
    }
}