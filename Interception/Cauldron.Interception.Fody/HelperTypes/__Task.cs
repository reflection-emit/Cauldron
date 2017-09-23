using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __Task : HelperTypeBase
    {
        public __Task(Builder builder) : base(builder, "System.Threading.Tasks.Task")
        {
            this.GetException = this.type.GetMethod("get_Exception");
        }

        public Method GetException { get; private set; }
    }
}