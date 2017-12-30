using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1")]
    public sealed class __AsyncTaskMethodBuilder_1 : HelperTypeBase<__AsyncTaskMethodBuilder_1>
    {
        [HelperTypeMethod("get_Task")]
        public Method GetTask { get; private set; }

        [HelperTypeMethod("SetResult", 1)]
        public Method SetResult { get; private set; }
    }
}