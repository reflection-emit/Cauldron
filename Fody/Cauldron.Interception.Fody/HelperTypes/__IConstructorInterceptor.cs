using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __IConstructorInterceptor : HelperTypeBase
    {
        public __IConstructorInterceptor(Builder builder) : base(builder, "Cauldron.Interception.IConstructorInterceptor")
        {
            this.OnEnter = this.type.GetMethod("OnEnter", 4);
            this.OnException = this.type.GetMethod("OnException", 1);
            this.OnExit = this.type.GetMethod("OnExit");
            this.OnBeforeInitialization = this.type.GetMethod("OnBeforeInitialization", 3);
        }

        public Method OnBeforeInitialization { get; private set; }
        public Method OnEnter { get; private set; }
        public Method OnException { get; private set; }
        public Method OnExit { get; private set; }
    }
}