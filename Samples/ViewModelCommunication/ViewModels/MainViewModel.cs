using Couldron;
using Couldron.Messaging;
using Couldron.ViewModels;
using System.Collections.ObjectModel;

namespace ViewModelCommunication.ViewModels
{
    public class MainViewModel : ViewModelBase
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

        public void OnCloseTab(CloseTabMessage message)
        {
            this.Animals.Remove(message.Sender as AnimalViewModel);
        }
    }
}