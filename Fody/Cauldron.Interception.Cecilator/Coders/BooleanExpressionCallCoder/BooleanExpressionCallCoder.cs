using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionCallCoder : ContextCoder
    {
        internal readonly Method calledMethod;
        internal readonly object instance;
        internal readonly object[] parameters;
        internal BuilderType castToType = null;

        internal BooleanExpressionCallCoder(Coder coder, object instance, Method calledMethod, object[] parameters) : base(coder)
        {
            this.parameters = parameters;
            this.calledMethod = calledMethod;
            this.instance = instance;
        }

        internal BooleanExpressionCallCoder(Coder coder, Instruction jumpTarget, object instance, Method calledMethod, object[] parameters) : base(coder, jumpTarget)
        {
            this.parameters = parameters;
            this.calledMethod = calledMethod;
            this.instance = instance;
        }
    }
}