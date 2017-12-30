using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.XAML.PropertyIsChangedEventArgs")]
    public sealed class __PropertyIsChangedEventArgs : HelperTypeBase<__PropertyIsChangedEventArgs>
    {
        [HelperTypeMethod(".ctor", 3)]
        public Method Ctor { get; private set; }
    }
}