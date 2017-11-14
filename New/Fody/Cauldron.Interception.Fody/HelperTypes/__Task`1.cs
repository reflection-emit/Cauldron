using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __Task_1 : HelperTypeBase
    {
        public __Task_1(Builder builder) : base(builder, "System.Threading.Tasks.Task`1")
        {
            this.GetResult = this.type.GetMethod("get_Result");
            this.FromResult = this.type.GetMethod("FromResult", 1);
        }

        public Method FromResult { get; private set; }
        public Method GetResult { get; private set; }
    }
}