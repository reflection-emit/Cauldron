using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("System.Runtime.CompilerServices.AsyncTaskMethodBuilder")]
    public sealed class __AsyncTaskMethodBuilder : HelperTypeBase<__AsyncTaskMethodBuilder>
    {
        [HelperTypeMethod("get_Task")]
        public Method GetTask { get; private set; }
    }
}