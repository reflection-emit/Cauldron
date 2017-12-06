using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __AsyncTaskMethodBuilder : HelperTypeBase
    {
        public __AsyncTaskMethodBuilder(Builder builder) : base(builder, "System.Runtime.CompilerServices.AsyncTaskMethodBuilder")
        {
            this.GetTask = this.type.GetMethod("get_Task");
        }

        public Method GetTask { get; private set; }
    }
}