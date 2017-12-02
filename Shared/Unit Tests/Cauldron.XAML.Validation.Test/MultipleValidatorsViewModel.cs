using Cauldron.XAML.Validation.ViewModels;

namespace Cauldron.XAML.Validation.Test
{
    public class MultipleValidatorsViewModel : ValidatableViewModelBase
    {
        [IsMandatory("")]
        [StringLength(20, "")]
        public string TestMethod { get; set; }
    }
}