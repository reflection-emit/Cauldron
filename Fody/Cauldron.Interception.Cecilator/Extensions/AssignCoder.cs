using Mono.Cecil;
using System.Collections.Generic;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public interface IAssignCoder
    {
        TypeReference TargetType { get; }

        Coder Set(object value);

        void Store();
    }

    public abstract class AssignCoder<T> : Coder, IAssignCoder
    {
        internal readonly AssignInstructionType instructionType;
        internal readonly List<T> target = new List<T>();

        internal AssignCoder(Coder coder, T target, AssignInstructionType instructionType) : base(coder.method, coder.instructions)
        {
            this.target.Add(target);
            this.instructionType = instructionType;
        }

        internal AssignCoder(Coder coder, IEnumerable<T> targets, AssignInstructionType instructionType) : base(coder.method, coder.instructions)
        {
            this.target.AddRange(targets);
            this.instructionType = instructionType;
        }

        public abstract TypeReference TargetType { get; }

        public override int GetHashCode() => this.instructions.GetHashCode() ^ this.method.GetHashCode() ^ this.instructions.GetHashCode();

        public Coder Set(object value)
        {
            if (this.instructionType == AssignInstructionType.Load)
                method.Log(LogTypes.Warning, sequencePoint: null, arg: "This could provoke error 0x8013184B. Use Assign() instead of Load if you want to set a new value to a field, property or variable.");

            var inst = this.AddParameter(this.processor, this.TargetType, value);
            this.instructions.Append(inst.Instructions);

            this.StoreCall();
            return new Coder(this.method, this.instructions);
        }

        void IAssignCoder.Store() => this.StoreCall();

        protected abstract void StoreCall();
    }
}