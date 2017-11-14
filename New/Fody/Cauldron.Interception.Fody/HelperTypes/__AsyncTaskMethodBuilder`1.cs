using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __AsyncTaskMethodBuilder_1 : HelperTypeBase
    {
        public __AsyncTaskMethodBuilder_1(Builder builder) : base(builder, "System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1")
        {
            this.GetTask = this.type.GetMethod("get_Task");
            this.SetResult = this.type.GetMethod("SetResult", 1);
        }

        public Method GetTask { get; private set; }
        public Method SetResult { get; private set; }
    }
}