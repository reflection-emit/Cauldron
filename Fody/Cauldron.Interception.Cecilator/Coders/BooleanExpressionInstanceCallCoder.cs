using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class BooleanExpressionInstanceCallCoder : BooleanExpressionContextCoder,
        IRelationalOperators<BooleanExpressionResultCoder, BooleanExpressionCoder, BooleanExpressionInstanceCallCoder>
    {
        internal readonly Method calledMethod;
        internal readonly object instance;
        internal readonly object[] parameters;

        internal BooleanExpressionInstanceCallCoder(Coder coder, object instance, Method calledMethod, object[] parameters) : base(coder)
        {
            this.parameters = parameters;
            this.calledMethod = calledMethod;
            this.instance = instance;
        }

        internal BooleanExpressionInstanceCallCoder(BooleanExpressionFieldInstancedCoder coder, Method calledMethod, object[] parameters) : base(coder.coder, coder.jumpTarget)
        {
            this.parameters = parameters;
            this.calledMethod = calledMethod;
            this.instance = coder.target;
        }

        internal BooleanExpressionInstanceCallCoder(BooleanExpressionInstanceCallCoder coder, Method calledMethod, object[] parameters) : base(coder.coder, coder.jumpTarget)
        {
            this.parameters = parameters;
            this.calledMethod = calledMethod;
            this.instance = coder.instance;
        }

        public BooleanExpressionResultCoder EqualsTo(object value) => this.Convert().EqualsTo(value);

        public BooleanExpressionResultCoder EqualsTo(Func<BooleanExpressionCoder, BooleanExpressionInstanceCallCoder> call)
            => BooleanExpressionCallCoder.EqualsToInternal(this.Convert(),
                new Func<BooleanExpressionCoder, BooleanExpressionCallCoder>(x => call(x).Convert()), false);

        public BooleanExpressionResultCoder EqualsTo(Field field) => this.Convert().EqualsTo(field);

        public BooleanExpressionResultCoder Is(Type type) => this.Convert().Is(type);

        public BooleanExpressionResultCoder Is(BuilderType type) => this.Convert().Is(type);

        public BooleanExpressionResultCoder IsFalse() => this.Convert().IsFalse();

        public BooleanExpressionResultCoder IsNotNull() => this.Convert().IsNotNull();

        public BooleanExpressionResultCoder IsNull() => this.Convert().IsNull();

        public BooleanExpressionResultCoder IsTrue() => this.Convert().IsTrue();

        public BooleanExpressionResultCoder NotEqualsTo(Field field) => this.Convert().NotEqualsTo(field);

        public BooleanExpressionResultCoder NotEqualsTo(object value) => this.Convert().NotEqualsTo(value);

        internal BooleanExpressionCallCoder Convert() => new BooleanExpressionCallCoder(this.coder, this.jumpTarget, this.instance, this.calledMethod, this.parameters);
    }
}