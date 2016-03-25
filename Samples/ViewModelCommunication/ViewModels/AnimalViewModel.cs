using Couldron.Aspects;
using Couldron.Messaging;
using Couldron.ViewModels;
using System;

namespace ViewModelCommunication.ViewModels
{
    public class AnimalViewModel : ViewModelBase, IClose
    {
        public AnimalViewModel(string animalName)
        {
            this.Name = animalName;
        }

        [NotifyPropertyChanged]
        public string Name { get; set; }

        public void Close()
        {
            MessageManager.Message(new CloseTabMessage(this));
        }
    }
}