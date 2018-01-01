using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("System.Threading.Tasks.Task`1", "System.Threading.Tasks.Task`1", "system.threading.tasks")]
    public sealed class __Task_1 : HelperTypeBase<__Task_1>
    {
        [HelperTypeMethod("FromResult", 1)]
        public Method FromResult { get; private set; }

        [HelperTypeMethod("get_Result")]
        public Method GetResult { get; private set; }
    }
}