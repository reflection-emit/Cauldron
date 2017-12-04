using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __Application : HelperTypeBase
    {
        public __Application(Builder builder) : base(builder, "System.Windows.Application")
        {
            this.LoadComponent = this.type.GetMethod("LoadComponent", true, "System.Object", "System.Uri");
            this.Current = this.type.GetMethod("get_Current", 0);
            this.Resources = this.type.GetMethod("get_Resources", 0);
        }

        public Method Current { get; private set; }
        public Method LoadComponent { get; private set; }
        public Method Resources { get; private set; }
    }
}