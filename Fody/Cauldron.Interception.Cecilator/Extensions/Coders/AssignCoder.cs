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

        public void InstructionDebug() => this.coder.InstructionDebug();

        internal abstract void StoreCall();
    }
}