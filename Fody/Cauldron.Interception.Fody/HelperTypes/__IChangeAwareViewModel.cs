using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __IChangeAwareViewModel : HelperTypeBase
    {
        public __IChangeAwareViewModel(Builder builder) : base(builder, "Cauldron.XAML.ViewModels.IChangeAwareViewModel")
        {
            this.RaisePropertyChanged = this.type.GetMethod("RaisePropertyChanged", 3);
            this.PropertyIsChangedEventArgs = new __PropertyIsChangedEventArgs(builder);
        }

        public __PropertyIsChangedEventArgs PropertyIsChangedEventArgs { get; private set; }

        public Method RaisePropertyChanged { get; private set; }

        public Field GetChanged(BuilderType builderType) => builderType.GetField("Changed", false);

        public Field GetIsChangedChanged(BuilderType builderType) => builderType.GetField("IsChangedChanged", false);
    }
}