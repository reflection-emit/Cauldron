using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class BooleanExpressionInstanceCallCoder : ContextCoder, IRelationalOperators<BooleanExpressionResultCoder, BooleanExpressionCoder, BooleanExpressionInstanceCallCoder>
    {
        internal readonly Method calledMethod;
        internal readonly object instance;
        internal readonly object[] parameters;
        internal BuilderType castToType = null;

        internal BooleanExpressionInstanceCallCoder(Coder coder, object instance, Method calledMethod, object[] parameters) : base(coder)
        {
            this.parameters = parameters;
            this.calledMethod = calledMethod;
            this.instance = instance;
        }

        internal BooleanExpressionInstanceCallCoder(Coder coder, Instruction jumpTarget, object instance, Method calledMethod, object[] parameters) : base(coder, jumpTarget)
        {
            this.parameters = parameters;
            this.calledMethod = calledMethod;
            this.instance = instance;
        }

        public static implicit operator BooleanExpressionCallCoder(BooleanExpressionInstanceCallCoder value)
            => new BooleanExpressionCallCoder(value.coder, value.jumpTarget, value.instance, value.calledMethod, value.parameters)
            {
                castToType = value.castToType
            };

        public BooleanExpressionResultCoder EqualsTo(object value) => ((BooleanExpressionCallCoder)this).EqualsTo(value);

        public BooleanExpressionResultCoder EqualsTo(Func<BooleanExpressionCoder, BooleanExpressionInstanceCallCoder> call)
            => BooleanExpressionCallCoder.EqualsToInternal(this,
                new Func<BooleanExpressionCoder, BooleanExpressionCallCoder>(x => call(x)), false);

        public BooleanExpressionResultCoder EqualsTo(Field field) => ((BooleanExpressionCallCoder)this).EqualsTo(field);

        public BooleanExpressionResultCoder Is(Type type) => ((BooleanExpressionCallCoder)this).Is(type);

        public BooleanExpressionResultCoder Is(BuilderType type) => ((BooleanExpressionCallCoder)this).Is(type);

        public BooleanExpressionResultCoder IsFalse() => ((BooleanExpressionCallCoder)this).IsFalse();

        public BooleanExpressionResultCoder IsNotNull() => ((BooleanExpressionCallCoder)this).IsNotNull();

        public BooleanExpressionResultCoder IsNull() => ((BooleanExpressionCallCoder)this).IsNull();

        public BooleanExpressionResultCoder IsTrue() => ((BooleanExpressionCallCoder)this).IsTrue();

        public BooleanExpressionResultCoder NotEqualsTo(Field field) => ((BooleanExpressionCallCoder)this).NotEqualsTo(field);

        public BooleanExpressionResultCoder NotEqualsTo(object value) => ((BooleanExpressionCallCoder)this).NotEqualsTo(value);
    }
}