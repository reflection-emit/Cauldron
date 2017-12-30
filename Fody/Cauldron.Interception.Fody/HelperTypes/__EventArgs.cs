using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("System.EventArgs")]
    public sealed class __EventArgs : HelperTypeBase<__EventArgs>
    {
        [HelperTypeMethod(".ctor")]
        public Method Ctor { get; private set; }

        [HelperTypeField("Empty")]
        public Field Empty { get; private set; }
    }
}