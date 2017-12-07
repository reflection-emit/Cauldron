using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __PropertyIsChangedEventArgs : HelperTypeBase
    {
        public __PropertyIsChangedEventArgs(Builder builder) : base(builder, "Cauldron.XAML.PropertyIsChangedEventArgs")
        {
            this.Ctor = this.type.GetMethod(".ctor", 3);
        }

        public Method Ctor { get; private set; }
    }
}