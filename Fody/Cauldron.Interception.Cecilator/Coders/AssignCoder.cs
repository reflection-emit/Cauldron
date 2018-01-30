using Mono.Cecil;

namespace Cauldron.Interception.Cecilator.Coders
{
    public abstract class AssignCoder<T>
    {
        internal readonly Coder coder;
        internal readonly T target;

        internal AssignCoder(Coder coder, T target)
        {
            this.coder = coder;
            this.target = target;
        }

        public abstract TypeReference TargetType { get; }

        public void InstructionDebug() => this.coder.InstructionDebug();

        internal abstract void StoreCall();
    }
}