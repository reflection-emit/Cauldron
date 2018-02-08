using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public class BooleanExpressionFieldCoder :
        CoderBase<BooleanExpressionFieldCoder, BooleanExpressionCoder>,
        ICallMethod<BooleanExpressionCallCoder>,
        IBinaryOperators<BooleanExpressionFieldCoder>,
        IFieldOperations<BooleanExpressionFieldCoder>,
        ICasting<BooleanExpressionFieldCoder>
    {
        private readonly BuilderType builderType;

        internal BooleanExpressionFieldCoder(InstructionBlock instructionBlock, BuilderType builderType) : base(instructionBlock) => this.builderType = builderType;

        public override BooleanExpressionCoder End => new BooleanExpressionCoder(this);

        public static implicit operator InstructionBlock(BooleanExpressionFieldCoder coder) => coder.instructions;

        #region Call Methods

        public BooleanExpressionCallCoder Call(Method method)
        {
            this.InternalCall(null, method);
            return new BooleanExpressionCallCoder(this, method.ReturnType);
        }

        public BooleanExpressionCallCoder Call(Method method, params object[] parameters)
        {
            this.InternalCall(null, method, parameters);
            return new BooleanExpressionCallCoder(this, method.ReturnType);
        }

        public BooleanExpressionCallCoder Call(Method method, params Func<Coder, object>[] parameters)
        {
            this.InternalCall(null, method, this.CreateParameters(parameters));
            return new BooleanExpressionCallCoder(this, method.ReturnType);
        }

        #endregion Call Methods

        #region Field Operations

        public BooleanExpressionFieldCoder Load(Field field)
        {
            InstructionBlock.CreateCodeForFieldReference(this, field.FieldType, field, false);
            return new BooleanExpressionFieldCoder(this, field.FieldType);
        }

        public BooleanExpressionFieldCoder Load(Func<BuilderType, Field> field) => Load(field(this.builderType));

        public Coder SetValue(Field field, object value)
        {
            this.instructions.Append(InstructionBlock.SetValue(this, null, field, value));
            return new Coder(this);
        }

        public Coder SetValue(Field field, Func<Coder, object> value) => SetValue(field, value(this.NewCoder()));

        public Coder SetValue(Func<BuilderType, Field> field, object value) => this.SetValue(field(this.instructions.associatedMethod.type), value);

        public Coder SetValue(Func<BuilderType, Field> field, Func<Coder, object> value) => SetValue(field, value(this.NewCoder()));

        #endregion Field Operations

        #region Casting Operations

        public BooleanExpressionFieldCoder As(BuilderType type)
        {
            InstructionBlock.CastOrBoxValues(this, type);
            return new BooleanExpressionFieldCoder(this, type);
        }

        #endregion Casting Operations

        #region Binary Operators

        public BooleanExpressionFieldCoder And(Func<Coder, object> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            this.instructions.Append(InstructionBlock.CreateCode(this, this.builderType, other(this.NewCoder())));
            this.instructions.Emit(OpCodes.And);
            return this;
        }

        public BooleanExpressionFieldCoder And(Field field)
        {
            this.instructions.Append(InstructionBlock.CreateCode(this, this.builderType, field));
            this.instructions.Emit(OpCodes.And);
            return this;
        }

        public BooleanExpressionFieldCoder And(LocalVariable variable)
        {
            this.instructions.Append(InstructionBlock.CreateCode(this, this.builderType, variable));
            this.instructions.Emit(OpCodes.And);
            return this;
        }

        public BooleanExpressionFieldCoder And(ParametersCodeBlock arg)
        {
            this.instructions.Append(InstructionBlock.CreateCode(this, this.builderType, arg));
            this.instructions.Emit(OpCodes.And);
            return this;
        }

        public BooleanExpressionFieldCoder Invert()
        {
            this.instructions.Emit(OpCodes.Ldc_I4_0);
            this.instructions.Emit(OpCodes.Ceq);
            return this;
        }

        public BooleanExpressionFieldCoder Or(Field field)
        {
            this.instructions.Append(InstructionBlock.CreateCode(this, this.builderType, field));
            this.instructions.Emit(OpCodes.And);
            return this;
        }

        public BooleanExpressionFieldCoder Or(LocalVariable variable)
        {
            this.instructions.Append(InstructionBlock.CreateCode(this, this.builderType, variable));
            this.instructions.Emit(OpCodes.And);
            return this;
        }

        public BooleanExpressionFieldCoder Or(Func<Coder, object> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            this.instructions.Append(InstructionBlock.CreateCode(this, this.builderType, other(this.NewCoder())));
            this.instructions.Emit(OpCodes.Or);
            return this;
        }

        public BooleanExpressionFieldCoder Or(ParametersCodeBlock arg)
        {
            this.instructions.Append(InstructionBlock.CreateCode(this, this.builderType, arg));
            this.instructions.Emit(OpCodes.And);
            return this;
        }

        #endregion Binary Operators
    }
}