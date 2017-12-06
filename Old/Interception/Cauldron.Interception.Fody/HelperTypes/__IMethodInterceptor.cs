using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __IMethodInterceptor : HelperTypeBase
    {
        public __IMethodInterceptor(Builder builder) : base(builder, "Cauldron.Interception.IMethodInterceptor")
        {
            this.OnEnter = this.type.GetMethod("OnEnter", 4);
            this.OnException = this.type.GetMethod("OnException", 1);
            this.OnExit = this.type.GetMethod("OnExit");
        }

        public Method OnEnter { get; private set; }
        public Method OnException { get; private set; }
        public Method OnExit { get; private set; }
    }
}