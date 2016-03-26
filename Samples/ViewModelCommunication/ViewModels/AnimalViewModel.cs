using Couldron;
using Couldron.Aspects;
using Couldron.Messaging;
using Couldron.ViewModels;
using System.Windows.Input;

namespace ViewModelCommunication.ViewModels
{
    public class AnimalViewModel : ViewModelBase, IClose
    {
        public AnimalViewModel(string animalName)
        {
            this.Name = animalName;

            this.ClearCommand = new RelayCommand(this.ClearAction);
        }

        public ICommand ClearCommand { get; private set; }

        [NotifyPropertyChanged]
        public string Name { get; set; }

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