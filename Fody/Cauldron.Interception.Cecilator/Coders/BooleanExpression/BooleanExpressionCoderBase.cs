using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public abstract class BooleanExpressionCoderBase<TSelf, TMaster> :
        BooleanExpressionCoderBase,
        IRelationalOperators,
        IMathOperators<TSelf>
        where TSelf : BooleanExpressionCoderBase<TSelf, TMaster>
        where TMaster : BooleanExpressionCoderBase

    {
        protected BooleanExpressionCoderBase(
            InstructionBlock instructionBlock,
            (Instruction beginning, Instruction ending)? jumpTargets = null,
            BuilderType builderType = null) :
            base(instructionBlock, jumpTargets, builderType)
        {
        }

        protected BooleanExpressionCoderBase(BooleanExpressionCoderBase self, BuilderType builderType) : base(self.instructions, self.jumpTargets, builderType)
        {
        }

        public abstract TMaster End { get; }

        public TSelf Append(TSelf coder)
        {
            this.instructions.Append(instructions);
            return this as TSelf;
        }

        public TSelf Append(InstructionBlock instructionBlock)
        {
            this.instructions.Append(instructionBlock);
            return this as TSelf;
        }

        public TSelf ArrayElement(int index)
        {
            this.instructions.Append(InstructionBlock.CreateCode(this.instructions, null, index));
            this.instructions.Emit(OpCodes.Ldelem_Ref);

            return this as TSelf;
        }

        /// <summary>
        /// Duplicates the last value in the stack
        /// </summary>
        /// <returns></returns>
        public TSelf Duplicate()
        {
            this.instructions.Emit(OpCodes.Dup);
            return (TSelf)this;
        }

        /// <summary>
        /// Negates a value e.g. 1 to -1
        /// </summary>
        /// <returns></returns>
        public TSelf Negate()
        {
            this.instructions.Emit(OpCodes.Neg);
            return (TSelf)this;
        }

        #region Relational Operations

        public BooleanExpressionResultCoder Is(Func<Coder, object> value) => this.Is(value(this.NewCoder()));

        public BooleanExpressionResultCoder Is(Type type) => this.Is(type.ToBuilderType());

        public BooleanExpressionResultCoder Is(BuilderType type)
        {
            this.instructions.Emit(OpCodes.Isinst, type.Import().typeReference);
            this.instructions.Emit(OpCodes.Ldnull);
            this.instructions.Emit(OpCodes.Cgt_Un);
            this.instructions.Emit(OpCodes.Brfalse, this.jumpTargets.ending);

            return new BooleanExpressionResultCoder(this);
        }

        public BooleanExpressionResultCoder Is(object value)
        {
            this.IsIsInternal(value);
            this.instructions.Emit(OpCodes.Brfalse, this.jumpTargets.ending);

            return new BooleanExpressionResultCoder(this);
        }

        public BooleanExpressionResultCoder IsNot(object value)
        {
            this.IsIsInternal(value);
            this.instructions.Emit(OpCodes.Brtrue, this.jumpTargets.ending);

            return new BooleanExpressionResultCoder(this);
        }

        public BooleanExpressionResultCoder IsNot(Type type) => this.IsNot(type.ToBuilderType());

        public BooleanExpressionResultCoder IsNot(BuilderType type)
        {
            this.instructions.Emit(OpCodes.Isinst, type.Import().typeReference);
            this.instructions.Emit(OpCodes.Ldnull);
            this.instructions.Emit(OpCodes.Cgt_Un);
            this.instructions.Emit(OpCodes.Brtrue, this.jumpTargets.ending);

            return new BooleanExpressionResultCoder(this);
        }

        public BooleanExpressionResultCoder IsNot(Func<Coder, object> value) => this.IsNot(value(this.NewCoder()));

        public BooleanExpressionResultCoder IsNotNull() => this.IsNot((object)null);

        public BooleanExpressionResultCoder IsNull() => this.Is((object)null);

        private void IsIsInternal(object value)
        {
            switch (value)
            {
                case null: this.instructions.Emit(OpCodes.Ldnull); this.instructions.Emit(OpCodes.Ceq); break;
                case bool o:
                    if (this.instructions.Count > 1 && this.instructions.LastElements(2).With(x => x[0].OpCode == OpCodes.Ldc_I4_0 && x[1].OpCode == OpCodes.Ceq))
                    {
                        // We are inverting the compare to optimize the code :)
                        var lastElements = this.instructions.LastElements(2);
                        lastElements[0].OpCode = o ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1;
                    }
                    else
                    {
                        InstructionBlock.CastOrBoxValues(this.instructions, BuilderType.Boolean);
                        this.instructions.Emit(o ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                        this.instructions.Emit(OpCodes.Ceq);
                    }
                    break;

                case Field field: InstructionBlock.AreEqualInternalWithoutJump(this.instructions, field.FieldType, value); break;
                case FieldDefinition field: InstructionBlock.AreEqualInternalWithoutJump(this.instructions, field.FieldType.ToBuilderType(), value); break;
                case FieldReference field: InstructionBlock.AreEqualInternalWithoutJump(this.instructions, field.FieldType.ToBuilderType(), value); break;
                case ParameterDefinition arg: InstructionBlock.AreEqualInternalWithoutJump(this.instructions, arg.ParameterType.ToBuilderType(), value); break;
                case ParameterReference arg: InstructionBlock.AreEqualInternalWithoutJump(this.instructions, arg.ParameterType.ToBuilderType(), value); break;
                case ParametersCodeBlock arg: InstructionBlock.AreEqualInternalWithoutJump(this.instructions, arg.GetTargetType(this.instructions.associatedMethod).Item1, value); break;
                case LocalVariable var: InstructionBlock.AreEqualInternalWithoutJump(this.instructions, var.Type, value); break;
                case VariableDefinition var: InstructionBlock.AreEqualInternalWithoutJump(this.instructions, var.VariableType.ToBuilderType(), value); break;
                case VariableReference var: InstructionBlock.AreEqualInternalWithoutJump(this.instructions, var.VariableType.ToBuilderType(), value); break;
                case BuilderType o: InstructionBlock.AreEqualInternalWithoutJump(this.instructions, BuilderType.Type, value); break;
                case TypeDefinition o: InstructionBlock.AreEqualInternalWithoutJump(this.instructions, BuilderType.Type, value); break;
                case TypeReference o: InstructionBlock.AreEqualInternalWithoutJump(this.instructions, BuilderType.Type, value); break;
                case Method o: InstructionBlock.AreEqualInternalWithoutJump(this.instructions, BuilderType.MethodBase, value); break;
                case CoderBase o: InstructionBlock.AreEqualInternalWithoutJump(this.instructions, null, value); break;
                default: InstructionBlock.AreEqualInternalWithoutJump(this.instructions, value.GetType().ToBuilderType(), value); break;
            }
        }

        #endregion Relational Operations
    }

    public abstract class BooleanExpressionCoderBase : CoderBase
    {
        internal readonly BuilderType builderType;
        internal readonly (Instruction beginning, Instruction ending) jumpTargets;

        protected BooleanExpressionCoderBase(InstructionBlock instructionBlock, (Instruction beginning, Instruction ending)? jumpTargets = null, BuilderType builderType = null) : base(instructionBlock)
        {
            this.jumpTargets = jumpTargets ?? (instructionBlock.ilprocessor.Create(OpCodes.Nop), instructionBlock.ilprocessor.Create(OpCodes.Nop));
            this.builderType = builderType?.Import() ?? instructionBlock.associatedMethod.OriginType;
        }
    }
}