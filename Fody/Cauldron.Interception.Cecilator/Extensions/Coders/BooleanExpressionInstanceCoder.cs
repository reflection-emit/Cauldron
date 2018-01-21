using Mono.Cecil;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public abstract class BooleanExpressionInstanceCoder<T> : BooleanExpressionCoder
    {
        internal readonly T target;

        internal BooleanExpressionInstanceCoder(Coder coder, T target) : base(coder)
        {
            this.target = target;
        }

        public abstract TypeReference TargetType { get; }

        public static explicit operator Coder(BooleanExpressionInstanceCoder<T> coder) => coder.coder;

        public override int GetHashCode()
            =>
            (this.target?.GetHashCode() ?? 22) ^
            this.coder.instructions.GetHashCode() ^
            this.coder.method.GetHashCode() ^
            this.coder.instructions.GetHashCode();

        public void InstructionDebug() => this.coder.InstructionDebug();
    }
}