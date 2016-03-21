using Couldron.Aspects;
using Couldron.Validation;
using Couldron.ViewModels;
using System;

namespace Couldron.Test.Resources
{
    public class BirdViewModel : ViewModelBase
    {
        [NotifyPropertyChanged]
        public double FlightHeight { get; set; }

        [NotifyPropertyChanged]
        public string Name { get; set; }

        [NotifyPropertyChanged(nameof(FlightHeight))]
        public double Speed { get; set; }
    }

    public class SparrowViewModel : ValidatableViewModelBase
    {
        [StringLength(5, 10, "key")]
        [NotifyPropertyChanged]
        public string Caption { get; set; }

        [IsMandatory("mandatory")]
        [NotifyPropertyChanged]
        public string Name { get; set; }

        [Equality(nameof(Password2), "password does not match")]
        [NotifyPropertyChanged]
        public string Password { get; set; }

        [Equality(nameof(Password), "password does not match 2")]
        [NotifyPropertyChanged]
        public string Password2 { get; set; }

        [StringLength(10, "key")]
        [NotifyPropertyChanged]
        public string Text { get; set; }
    }
}