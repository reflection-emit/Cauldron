using Cauldron.Interception.Cecilator.Extensions;
using System;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Coders
{
    public abstract class CoderBase
    {
        internal readonly InstructionBlock instructions;

        protected CoderBase(InstructionBlock instructionBlock) => this.instructions = instructionBlock;
    }

    public abstract class CoderBase<TSelf, TMaster> : CoderBase
        where TSelf : CoderBase<TSelf, TMaster>
        where TMaster : CoderBase
    {
        protected CoderBase(InstructionBlock instructionBlock) : base(instructionBlock)
        {
        }

        public abstract TMaster End { get; }

        public TSelf Append(TSelf coder)
        {
            this.instructions.Append(instructions.instructions);
            return this as TSelf;
        }

        public TSelf Append(InstructionBlock instructionBlock)
        {
            this.instructions.Append(instructionBlock);
            return this as TSelf;
        }

        public void InstructionDebug() => this.instructions.associatedMethod.Log(LogTypes.Info, this.instructions);

        public override string ToString() => this.instructions.associatedMethod.Fullname;

        protected object[] CreateParameters(Func<Coder, object>[] parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            return parameters.Select(x => x(this.instructions.associatedMethod.NewCoder())).ToArray();
        }

        protected void InternalCall(object instance, Method method) =>
            InternalCall(instance, method, new object[0]);

        protected void InternalCall(object instance, Method method, object[] parameters) =>
            this.instructions.Append(InstructionBlock.Call(this.instructions, instance, method, parameters));
    }
}