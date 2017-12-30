using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("System.Windows.Application")]
    public sealed class __Application : HelperTypeBase<__Application>
    {
        [HelperTypeMethod("get_Current")]
        public Method Current { get; private set; }

        [HelperTypeMethod("LoadComponent", "System.Object", "System.Uri")]
        public Method LoadComponent { get; private set; }

        [HelperTypeMethod("get_Resources")]
        public Method Resources { get; private set; }
    }
}