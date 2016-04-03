using Couldron;
using Couldron.Core;
using Couldron.Messaging;
using Couldron.Validation;
using Couldron.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ViewModelCommunication.ViewModels
{
    public class AnimalViewModel : ValidatableViewModelBase, IClose
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

        [IsMandatory("This is mandatory")]
        public string Name { get; set; }

        public ObservableCollection<AnimalProperty> Properties { get; private set; }

        public void Close()
        {
            this.Validate();

            if (!this.HasErrors)
                MessageManager.Message(new CloseTabMessage(this));
        }

        private void ClearAction()
        {
            this.Name = string.Empty;
        }
    }
}