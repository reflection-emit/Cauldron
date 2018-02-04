using Mono.Cecil;

namespace Cauldron.Interception.Cecilator.Coders
{
    public abstract class BooleanExpressionInstanceCoder<T> : BooleanExpressionCoder
    {
        internal readonly T target;

        internal BooleanExpressionInstanceCoder(Coder coder, T target) : base(coder) => this.target = target;

        public abstract TypeReference TargetType { get; }

        internal BooleanExpressionCoder ToBooleanExpressionCoder()
        {
            this.coder.Append(this.coder.AddParameter(this.coder.processor, this.TargetType, this.target).Instructions);
            this.coder.Append(this.ImplementOperations());
            return this as BooleanExpressionCoder;
        }
    }
}