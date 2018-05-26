using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("System.Threading.Monitor")]
    public sealed class __Monitor : HelperTypeBase<__Monitor>
    {
        [HelperTypeMethod("Enter", 2)]
        public Method Enter { get; private set; }

        [HelperTypeMethod("Exit", 1)]
        public Method Exit { get; private set; }
    }
}