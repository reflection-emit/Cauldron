using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class ArgCoder :
        CoderBase<ArgCoder, Coder>,
        ICallMethod<CallCoder>,
        IExitOperators,
        IFieldOperationsExtended<FieldCoder>,
        IBinaryOperators<ArgCoder>,
        ICasting<ArgCoder>
    {
        private readonly BuilderType builderType;

        internal ArgCoder(InstructionBlock instructionBlock, BuilderType builderType) : base(instructionBlock) => this.builderType = builderType;

        public override Coder End => new Coder(this);

        public static implicit operator InstructionBlock(ArgCoder coder) => coder.instructions;

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
            this.ImplementReturn();
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

        public ArgCoder As(BuilderType type)
        {
            InstructionBlock.CastOrBoxValues(this, type);
            return new ArgCoder(this, type);
        }

        #endregion Casting Operations

        #region Binary Operators

        public ArgCoder And(Func<Coder, object> other)
        {
            this.And(this.builderType, other);
            return this;
        }

        public ArgCoder And(Field field)
        {
            this.And(this.builderType, field);
            return this;
        }

        public ArgCoder And(LocalVariable variable)
        {
            this.And(this.builderType, variable);
            return this;
        }

        public ArgCoder And(ParametersCodeBlock arg)
        {
            this.And(this.builderType, arg);
            return this;
        }

        public ArgCoder Invert()
        {
            this.InvertInternal();
            return this;
        }

        public ArgCoder Or(Field field)
        {
            this.Or(this.builderType, field);
            return this;
        }

        public ArgCoder Or(LocalVariable variable)
        {
            this.Or(this.builderType, variable);
            return this;
        }

        public ArgCoder Or(Func<Coder, object> other)
        {
            this.Or(this.builderType, other);
            return this;
        }

        public ArgCoder Or(ParametersCodeBlock arg)
        {
            this.Or(this.builderType, arg);
            return this;
        }

        #endregion Binary Operators

        public Coder StoreElement(object element, int index)
        {
            this.StoreElementInternal(this.builderType, element, index);
            return new Coder(this.instructions);
        }
    }
}