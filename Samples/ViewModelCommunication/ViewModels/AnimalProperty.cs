using Cauldron.Validation;
using Cauldron.ViewModels;

namespace ViewModelCommunication.ViewModels
{
    public class AnimalProperty : ValidatableViewModelBase
    {
        [IsMandatory("huhu")]
        public string Name { get; set; }

        public int Value { get; set; }
    }
}