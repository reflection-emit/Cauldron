using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public abstract class BooleanExpressionInstanceCoder<T> : BooleanExpressionCoder
    {
        internal readonly T target;
        internal bool invert;
        internal bool negate;

        internal BooleanExpressionInstanceCoder(Coder coder, T target) : base(coder) => this.target = target;

        public abstract TypeReference TargetType { get; }

        internal BooleanExpressionCoder Implement()
        {
            this.coder.Append(this.coder.AddParameter(this.coder.processor, this.TargetType, this.target).Instructions);

            if (this.negate)
                this.coder.instructions.Append(this.coder.processor.Create(OpCodes.Neg));

            if (this.invert)
            {
                this.coder.instructions.Append(this.coder.processor.Create(OpCodes.Ldc_I4_0));
                this.coder.instructions.Append(this.coder.processor.Create(OpCodes.Ceq));
            }

            this.negate = false;
            this.invert = false;

            return this as BooleanExpressionCoder;
        }
    }
}