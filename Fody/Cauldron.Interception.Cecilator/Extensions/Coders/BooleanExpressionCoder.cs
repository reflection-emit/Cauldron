using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public class BooleanExpressionCallCoder : ContextCoder
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

    public class BooleanExpressionCoder : ContextCoder
    {
        internal BooleanExpressionCoder(Coder coder) : base(coder)
        {
        }

        internal BooleanExpressionCoder(Coder coder, Instruction jumpTarget) : base(coder, jumpTarget)
        {
        }

        internal BooleanExpressionCoder(BooleanExpressionCallCoder coder) : base(coder.coder, coder.jumpTarget)
        {
        }
    }
}