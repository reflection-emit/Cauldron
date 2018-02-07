namespace Cauldron.Interception.Cecilator.Coders
{
    internal class BooleanExpressionParameter
    {
        public readonly BuilderType targetType;
        public readonly object value;

        public BooleanExpressionParameter(object value, BuilderType targetType)
        {
            this.targetType = targetType ?? value.GetType().ToBuilderType();
            this.value = value;
        }
    }
}