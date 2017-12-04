using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __Application : HelperTypeBase
    {
        public __Application(Builder builder) : base(builder, "System.Windows.Application")
        {
            this.LoadComponent = this.type.GetMethod("LoadComponent", true, "System.Object", "System.Uri");
        }

        public Method LoadComponent { get; private set; }
    }
}