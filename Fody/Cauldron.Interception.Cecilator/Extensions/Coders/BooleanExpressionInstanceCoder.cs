using Mono.Cecil;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public abstract class BooleanExpressionInstanceCoder<T> : BooleanExpressionCoder
    {
        internal readonly T target;

        internal BooleanExpressionInstanceCoder(Coder coder, T target) : base(coder) => this.target = target;

        public abstract TypeReference TargetType { get; }
    }
}