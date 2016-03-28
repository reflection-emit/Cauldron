using Couldron.Aspects;
using Couldron.ViewModels;

namespace ViewModelCommunication.ViewModels
{
    public class AnimalProperty : ViewModelBase
    {
        [NotifyPropertyChanged]
        public string Name { get; set; }

        [NotifyPropertyChanged]
        public int Value { get; set; }
    }
}