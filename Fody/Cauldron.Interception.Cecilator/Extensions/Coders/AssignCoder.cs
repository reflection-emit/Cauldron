using Mono.Cecil;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public abstract class AssignCoder<T>
    {
        internal readonly Coder coder;
        internal readonly T target;

        internal AssignCoder(Coder coder, T target)
        {
            this.coder = new Coder(coder.method, coder.instructions);
            this.target = target;
        }

        public abstract TypeReference TargetType { get; }

        public static explicit operator Coder(AssignCoder<T> coder) => coder.coder;

        public override int GetHashCode()
            =>
            (this.target?.GetHashCode() ?? 22) ^
            this.coder.instructions.GetHashCode() ^
            this.coder.method.GetHashCode() ^
            this.coder.instructions.GetHashCode();

        public void InstructionDebug() => this.coder.InstructionDebug();

        internal abstract void StoreCall();
    }
}