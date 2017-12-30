using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("System.Threading.Tasks.Task")]
    public sealed class __Task : HelperTypeBase<__Task>
    {
        [HelperTypeMethod("get_Exception")]
        public Method GetException { get; private set; }
    }
}