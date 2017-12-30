using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.XAML.ViewModels.IChangeAwareViewModel")]
    public sealed class __IChangeAwareViewModel : HelperTypeBase<__IChangeAwareViewModel>
    {
        [HelperTypeMethod("get_IsChanged")]
        public Method IsChanged { get; private set; }

        public __PropertyIsChangedEventArgs PropertyIsChangedEventArgs => new __PropertyIsChangedEventArgs();

        [HelperTypeMethod("RaisePropertyChanged", 3)]
        public Method RaisePropertyChanged { get; private set; }

        public static Field GetChanged(BuilderType builderType) => builderType.GetField("Changed", false);

        public static Field GetIsChangedChanged(BuilderType builderType) => builderType.GetField("IsChangedChanged", false);
    }
}