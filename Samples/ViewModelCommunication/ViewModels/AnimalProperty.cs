using Couldron.Validation;
using Couldron.ViewModels;

namespace ViewModelCommunication.ViewModels
{
    public class AnimalProperty : ValidatableViewModelBase
    {
        [IsMandatory("huhu")]
        public string Name { get; set; }

        public int Value { get; set; }
    }
}