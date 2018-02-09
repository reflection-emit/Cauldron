using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class BooleanExpressionCallCoder :
        BooleanExpressionCoderBase<BooleanExpressionCallCoder, BooleanExpressionCoder>,
        IBinaryOperators<BooleanExpressionCallCoder>,
        ICallMethod<BooleanExpressionCallCoder>
    {
        internal BooleanExpressionCallCoder(BooleanExpressionCoderBase coder, BuilderType builderType) : base(coder, builderType)
        {
        }

        public override BooleanExpressionCoder End => new BooleanExpressionCoder(this);

        public static implicit operator InstructionBlock(BooleanExpressionCallCoder coder) => coder.instructions;

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

        #region Binary Operators

        public BooleanExpressionCallCoder And(Func<Coder, object> other)
        {
            this.And(this.builderType, other);
            return this;
        }

        public BooleanExpressionCallCoder And(Field field)
        {
            this.And(this.builderType, field);
            return this;
        }

        public BooleanExpressionCallCoder And(LocalVariable variable)
        {
            this.And(this.builderType, variable);
            return this;
        }

        public BooleanExpressionCallCoder And(ParametersCodeBlock arg)
        {
            this.And(this.builderType, arg);
            return this;
        }

        public BooleanExpressionCallCoder Invert()
        {
            this.InvertInternal();
            return this;
        }

        public BooleanExpressionCallCoder Or(Field field)
        {
            this.Or(this.builderType, field);
            return this;
        }

        public BooleanExpressionCallCoder Or(LocalVariable variable)
        {
            this.Or(this.builderType, variable);
            return this;
        }

        public BooleanExpressionCallCoder Or(Func<Coder, object> other)
        {
            this.Or(this.builderType, other);
            return this;
        }

        public BooleanExpressionCallCoder Or(ParametersCodeBlock arg)
        {
            this.Or(this.builderType, arg);
            return this;
        }

        #endregion Binary Operators
    }
}