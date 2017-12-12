using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public class __IViewModel : HelperTypeBase
    {
        public __IViewModel(Builder builder) : base(builder, "Cauldron.XAML.ViewModels.IViewModel")
        {
            this.IsLoading = this.type.GetMethod("get_IsLoading");
            this.RaisePropertyChanged = this.type.GetMethod("RaisePropertyChanged", true, "System.String");
        }

        public Method IsLoading { get; private set; }

        public Method RaisePropertyChanged { get; private set; }
    }
}