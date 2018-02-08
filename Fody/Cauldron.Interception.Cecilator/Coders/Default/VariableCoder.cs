using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class VariableCoder :
        CoderBase<VariableCoder, Coder>,
        ICallMethod<CallCoder>,
        IFieldOperationsExtended<FieldCoder>,
        IBinaryOperators<VariableCoder>,
        ICasting<VariableCoder>,
        IExitOperators
    {
        private readonly BuilderType builderType;

        internal VariableCoder(InstructionBlock instructionBlock, BuilderType builderType) : base(instructionBlock) => this.builderType = builderType;

        public override Coder End => new Coder(this);

        public static implicit operator InstructionBlock(VariableCoder coder) => coder.instructions;

        #region Call Methods

        public CallCoder Call(Method method)
        {
            this.InternalCall(null, method);
            return new CallCoder(this, method.ReturnType);
        }

        public CallCoder Call(Method method, params object[] parameters)
        {
            this.InternalCall(null, method, parameters);
            return new CallCoder(this, method.ReturnType);
        }

        public CallCoder Call(Method method, params Func<Coder, object>[] parameters)
        {
            this.InternalCall(null, method, this.CreateParameters(parameters));
            return new CallCoder(this, method.ReturnType);
        }

        #endregion Call Methods

        #region Exit Operators

        public Coder Return()
        {
            this.instructions.Emit(OpCodes.Ret);
            return new Coder(this);
        }

        #endregion Exit Operators

        #region Field Operations

        public FieldCoder Load(Field field)
        {
            InstructionBlock.CreateCodeForFieldReference(this, field.FieldType, field, false);
            return new FieldCoder(this, field.FieldType);
        }

        public FieldCoder Load(Func<BuilderType, Field> field) => Load(field(this.builderType));

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

        public VariableCoder As(BuilderType type)
        {
            InstructionBlock.CastOrBoxValues(this, type);
            return new VariableCoder(this, type);
        }

        #endregion Casting Operations

        #region Binary Operators

        public VariableCoder And(Func<Coder, object> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            this.instructions.Append(InstructionBlock.CreateCode(this, this.builderType, other(this.NewCoder())));
            this.instructions.Emit(OpCodes.And);
            return this;
        }

        public VariableCoder And(Field field)
        {
            this.instructions.Append(InstructionBlock.CreateCode(this, this.builderType, field));
            this.instructions.Emit(OpCodes.And);
            return this;
        }

        public VariableCoder And(LocalVariable variable)
        {
            this.instructions.Append(InstructionBlock.CreateCode(this, this.builderType, variable));
            this.instructions.Emit(OpCodes.And);
            return this;
        }

        public VariableCoder And(ParametersCodeBlock arg)
        {
            this.instructions.Append(InstructionBlock.CreateCode(this, this.builderType, arg));
            this.instructions.Emit(OpCodes.And);
            return this;
        }

        public VariableCoder Invert()
        {
            this.instructions.Emit(OpCodes.Ldc_I4_0);
            this.instructions.Emit(OpCodes.Ceq);
            return this;
        }

        public VariableCoder Or(Field field)
        {
            this.instructions.Append(InstructionBlock.CreateCode(this, this.builderType, field));
            this.instructions.Emit(OpCodes.And);
            return this;
        }

        public VariableCoder Or(LocalVariable variable)
        {
            this.instructions.Append(InstructionBlock.CreateCode(this, this.builderType, variable));
            this.instructions.Emit(OpCodes.And);
            return this;
        }

        public VariableCoder Or(Func<Coder, object> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            this.instructions.Append(InstructionBlock.CreateCode(this, this.builderType, other(this.NewCoder())));
            this.instructions.Emit(OpCodes.Or);
            return this;
        }

        public VariableCoder Or(ParametersCodeBlock arg)
        {
            this.instructions.Append(InstructionBlock.CreateCode(this, this.builderType, arg));
            this.instructions.Emit(OpCodes.And);
            return this;
        }

        #endregion Binary Operators
    }
}