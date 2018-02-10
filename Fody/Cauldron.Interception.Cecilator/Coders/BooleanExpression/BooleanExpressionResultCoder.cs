using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class BooleanExpressionResultCoder : BooleanExpressionCoderBase, IConditionalLogicalOperators
    {
        internal BooleanExpressionResultCoder(BooleanExpressionCoderBase coder) : base(coder.instructions, coder.jumpTargets, coder.builderType)
        {
        }

        public static implicit operator InstructionBlock(BooleanExpressionResultCoder coder) => coder.instructions;

        public BooleanExpressionResultCoder AndAnd(Func<BooleanExpressionCoder, BooleanExpressionResultCoder> other) => other(new BooleanExpressionCoder(this.instructions, this.jumpTargets));

        public BooleanExpressionResultCoder AndAnd(Field other) => this.AndAnd(x => x.Load(other).Is(true));

        public BooleanExpressionResultCoder AndAnd(LocalVariable other) => this.AndAnd(x => x.Load(other).Is(true));

        public BooleanExpressionResultCoder AndAnd(ParametersCodeBlock other) => this.AndAnd(x => x.Load(other).Is(true));

        public BooleanExpressionResultCoder OrOr(Field other) => this.OrOr(x => x.Load(other).Is(true));

        public BooleanExpressionResultCoder OrOr(LocalVariable other) => this.OrOr(x => x.Load(other).Is(true));

        public BooleanExpressionResultCoder OrOr(ParametersCodeBlock other) => this.OrOr(x => x.Load(other).Is(true));

        public BooleanExpressionResultCoder OrOr(Func<BooleanExpressionCoder, BooleanExpressionResultCoder> other)
        {
            if (this.instructions.Count == 0)
                throw new NotSupportedException();

            var lastIntructions = this.instructions.Last;

            if (lastIntructions.OpCode == OpCodes.Brtrue)
            {
                lastIntructions.OpCode = OpCodes.Brfalse;
                lastIntructions.Operand = this.jumpTargets.beginning;
            }
            else if (lastIntructions.OpCode == OpCodes.Brfalse)
            {
                lastIntructions.OpCode = OpCodes.Brtrue;
                lastIntructions.Operand = this.jumpTargets.beginning;
            }
            else
                throw new NotImplementedException("Unknown jump opcode");

            return other(new BooleanExpressionCoder(this.instructions, this.jumpTargets));
        }
    }
}