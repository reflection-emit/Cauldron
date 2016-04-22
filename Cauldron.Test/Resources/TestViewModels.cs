using Cauldron.Validation;
using Cauldron.ViewModels;
using PropertyChanged;

namespace Cauldron.Test.Resources
{
    public class BirdViewModel : ViewModelBase
    {
        public double FlightHeight { get; set; }

        public string Name { get; set; }

        [AlsoNotifyFor(nameof(FlightHeight))]
        public double Speed { get; set; }
    }

    public class SparrowViewModel : ValidatableViewModelBase
    {
        [StringLength(5, 10, "key")]
        public string Caption { get; set; }

        [IsMandatory("mandatory")]
        public string Name { get; set; }

        [Equality(nameof(Password2), "password does not match")]
        public string Password { get; set; }

        [Equality(nameof(Password), "password does not match 2")]
        public string Password2 { get; set; }

        [StringLength(10, "key")]
        public string Text { get; set; }
    }
}