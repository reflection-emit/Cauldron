using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.XAML.ViewModels.IViewModel")]
    public class __IViewModel : HelperTypeBase<__IViewModel>
    {
        [HelperTypeMethod("get_IsLoading")]
        public Method IsLoading { get; private set; }

        [HelperTypeMethod("RaisePropertyChanged", "System.String")]
        public Method RaisePropertyChanged { get; private set; }
    }
}