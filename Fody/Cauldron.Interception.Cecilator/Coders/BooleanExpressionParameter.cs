using Mono.Cecil;

namespace Cauldron.Interception.Cecilator.Coders
{
    internal class BooleanExpressionParameter
    {
        public readonly bool invert;
        public readonly bool negate;
        public readonly TypeReference targetType;
        public readonly object value;

        public BooleanExpressionParameter(object value, TypeReference targetType, bool invert, bool negate)
        {
            this.targetType = targetType ?? value.GetType().ToBuilderType().typeReference;
            this.value = value;
            this.invert = invert;
            this.negate = negate;
        }

        public BooleanExpressionParameter(BooleanExpressionFieldInstancedCoder coder)
        {
            this.targetType = coder.TargetType;
            this.value = coder.target;
            this.invert = coder.invert;
            this.negate = coder.negate;
        }

        public object GetValue()
        {
            if (!this.invert && !this.negate)
                return this.value;

            return null;
        }
    }
}