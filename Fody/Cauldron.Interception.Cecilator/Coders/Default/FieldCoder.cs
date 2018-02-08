using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class FieldCoder :
        CoderBase<FieldCoder, Coder>,
        ICallMethod<CallCoder>,
        IFieldOperationsExtended<FieldCoder>,
        ICasting<FieldCoder>,
        IExitOperators,
        IBinaryOperators<FieldCoder>
    {
        private readonly BuilderType builderType;

        internal FieldCoder(InstructionBlock instructionBlock, BuilderType builderType) : base(instructionBlock) => this.builderType = builderType;

        public override Coder End => new Coder(this);

        public static implicit operator InstructionBlock(FieldCoder coder) => coder.instructions;

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

        public FieldCoder As(BuilderType type)
        {
            InstructionBlock.CastOrBoxValues(this, type);
            return new FieldCoder(this, type);
        }

        #endregion Casting Operations

        #region Binary Operators

        public FieldCoder And(Func<Coder, object> other)
        {
            this.And(this.builderType, other);
            return this;
        }

        public FieldCoder And(Field field)
        {
            this.And(this.builderType, field);
            return this;
        }

        public FieldCoder And(LocalVariable variable)
        {
            this.And(this.builderType, variable);
            return this;
        }

        public FieldCoder And(ParametersCodeBlock arg)
        {
            this.And(this.builderType, arg);
            return this;
        }

        public FieldCoder Invert()
        {
            this.InvertInternal();
            return this;
        }

        public FieldCoder Or(Field field)
        {
            this.Or(this.builderType, field);
            return this;
        }

        public FieldCoder Or(LocalVariable variable)
        {
            this.Or(this.builderType, variable);
            return this;
        }

        public FieldCoder Or(Func<Coder, object> other)
        {
            this.Or(this.builderType, other);
            return this;
        }

        public FieldCoder Or(ParametersCodeBlock arg)
        {
            this.Or(this.builderType, arg);
            return this;
        }

        #endregion Binary Operators
    }
}