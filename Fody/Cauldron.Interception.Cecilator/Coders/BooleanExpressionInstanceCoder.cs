using Mono.Cecil;

namespace Cauldron.Interception.Cecilator.Coders
{
    public abstract class BooleanExpressionInstanceCoder<T> : BooleanExpressionCoder
    {
        internal readonly T target;

        internal BooleanExpressionInstanceCoder(Coder coder, T target) : base(coder) => this.target = target;

        public abstract BuilderType TargetType { get; }

        internal BooleanExpressionCoder ToBooleanExpressionCoder()
        {
            this.coder.instructions.Append(InstructionBlock.CreateCode(this.coder.instructions, this.TargetType, this.target));
            return this as BooleanExpressionCoder;
        }
    }
}