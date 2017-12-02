using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __IPropertyGetterInterceptor : HelperTypeBase
    {
        public __IPropertyGetterInterceptor(Builder builder) : base(builder, "Cauldron.Interception.IPropertyGetterInterceptor")
        {
            this.OnGet = this.type.GetMethod("OnGet", 2);
            this.OnException = this.type.GetMethod("OnException", 1);
            this.OnExit = this.type.GetMethod("OnExit");
        }

        public Method OnException { get; private set; }
        public Method OnExit { get; private set; }
        public Method OnGet { get; private set; }
    }
}