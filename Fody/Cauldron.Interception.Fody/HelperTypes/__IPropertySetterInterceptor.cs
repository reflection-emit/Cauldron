using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __IPropertySetterInterceptor : HelperTypeBase
    {
        public __IPropertySetterInterceptor(Builder builder) : base(builder, "Cauldron.Interception.IPropertySetterInterceptor")
        {
            this.OnSet = this.type.GetMethod("OnSet", 3);
            this.OnException = this.type.GetMethod("OnException", 1);
            this.OnExit = this.type.GetMethod("OnExit");
        }

        public Method OnException { get; private set; }
        public Method OnExit { get; private set; }
        public Method OnSet { get; private set; }
    }
}