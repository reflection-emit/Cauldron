using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public class ArgAssignCoder : ArgContextCoder
    {
        internal ArgAssignCoder(Coder coder, ParametersCodeBlock parametersCodeBlock) : base(coder, parametersCodeBlock)
        {
        }

        internal ArgAssignCoder(ArgAssignCoder coder, ParametersCodeBlock parametersCodeBlock) : base(coder.coder, parametersCodeBlock)
        {
        }

        public BuilderType TargetType => this.target?.GetTargetType(this.coder.instructions.associatedMethod)?.Item1;

        public FieldContextCoder As(BuilderType type)
        {
            this.coder.instructions.Append(InstructionBlock.CreateCode(this.coder, type, this.target));
            return new FieldContextCoder(this.coder, false, null);
        }

        public Coder Set(object value)
        {
            // value to assign
            this.coder.instructions.Append(InstructionBlock.CreateCode(this.coder, this.TargetType, value));

            // Store
            this.StoreCall();
            return this.coder;
        }

        public Coder Set(Func<Coder, object> valueToAssignToVariable)
        {
            // value to assign
            this.coder.instructions.Append(InstructionBlock.CreateCode(this.coder, this.TargetType, valueToAssignToVariable(coder.NewCoder())));

            // Store
            this.StoreCall();
            return coder;
        }

        private void StoreCall()
        {
            var result = this.target?.GetTargetType(this.coder.instructions.associatedMethod);

            if (result == null)
                return;

            this.coder.instructions.Emit(OpCodes.Starg, result.Item2);
        }
    }
}