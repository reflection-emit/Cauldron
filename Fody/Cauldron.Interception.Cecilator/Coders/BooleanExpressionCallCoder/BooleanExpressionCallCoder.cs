using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionCallCoder : BooleanExpressionContextCoder
    {
        internal readonly Method calledMethod;
        internal readonly object instance;
        internal readonly object[] parameters;

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

        public CodeBlock ToCodeBlock()
        {
            var coder = this.coder.NewCoder();

            coder.CallInternal(this.instance, this.calledMethod, OpCodes.Call, this.parameters);
            coder.instructions.Append(coder.AddParameter(coder.processor, null, this).Instructions);

            return new InstructionsCodeBlock(coder);
        }
    }
}