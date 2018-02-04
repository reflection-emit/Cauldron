using Mono.Cecil;

namespace Cauldron.Interception.Cecilator.Coders
{
    internal class BooleanExpressionParameter : BooleanExpressionContextCoder
    {
        public readonly TypeReference targetType;
        public readonly object value;

        public BooleanExpressionParameter(BooleanExpressionContextCoder contextCoder, object value, TypeReference targetType) : base(contextCoder)
        {
            this.targetType = targetType ?? value.GetType().ToBuilderType().typeReference;
            this.value = value;
        }

        public BooleanExpressionParameter(BooleanExpressionContextCoder contextCoder, object value) : base(contextCoder)
        {
            this.targetType = contextCoder.castToType.typeReference;
            this.value = value;
        }

        public BooleanExpressionParameter(BooleanExpressionFieldInstancedCoder coder) : base(coder)
        {
            this.targetType = coder.TargetType;
            this.value = coder.target;
        }

        public object GetValue()
        {
            if (!this.invert && !this.negate)
                return this.value;

            return null;
        }
    }
}