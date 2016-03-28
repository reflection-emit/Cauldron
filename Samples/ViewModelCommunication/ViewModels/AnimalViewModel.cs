using Couldron;
using Couldron.Aspects;
using Couldron.Core;
using Couldron.Messaging;
using Couldron.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ViewModelCommunication.ViewModels
{
    public class AnimalViewModel : ViewModelBase, IClose
    {
        public AnimalViewModel(string animalName)
        {
            this.Properties = new ObservableCollection<AnimalProperty>();

            this.Name = animalName;

            this.ClearCommand = new RelayCommand(this.ClearAction);

            this.Properties.Add(new AnimalProperty { Name = "Speed", Value = Randomizer.Next(2, 100) });
            this.Properties.Add(new AnimalProperty { Name = "Weight", Value = Randomizer.Next(30, 460) });
            this.Properties.Add(new AnimalProperty { Name = "Leg count", Value = Randomizer.Next(2, 8) });
            this.Properties.Add(new AnimalProperty { Name = "Population count", Value = Randomizer.Next(100, 100000000) });
            this.Properties.Add(new AnimalProperty { Name = "Deadliness", Value = Randomizer.Next(2, 100) });
            this.Properties.Add(new AnimalProperty { Name = "Health", Value = Randomizer.Next(2, 250) });
        }

        public ICommand ClearCommand { get; private set; }

        [NotifyPropertyChanged]
        public string Name { get; set; }

        public ObservableCollection<AnimalProperty> Properties { get; private set; }

        public void Close()
        {
            MessageManager.Message(new CloseTabMessage(this));
        }

        private void ClearAction()
        {
            this.Name = string.Empty;
        }
    }
}