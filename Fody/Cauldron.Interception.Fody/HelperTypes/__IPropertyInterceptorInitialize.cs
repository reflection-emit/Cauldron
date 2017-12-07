using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __IPropertyInterceptorInitialize : HelperTypeBase
    {
        public __IPropertyInterceptorInitialize(Builder builder) : base(builder, "Cauldron.Interception.IPropertyInterceptorInitialize")
        {
            this.OnInitialize = this.type.GetMethod("OnInitialize", 2);
        }

        public Method OnInitialize { get; private set; }
    }
}