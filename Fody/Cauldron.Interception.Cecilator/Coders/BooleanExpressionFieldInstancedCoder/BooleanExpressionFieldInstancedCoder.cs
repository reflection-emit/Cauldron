namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionFieldInstancedCoder : BooleanExpressionInstanceCoder<Field>
    {
        internal BooleanExpressionFieldInstancedCoder(BooleanExpressionCoder coder, Field target) :
            base(coder.coder, target)
        {
        }

        public override BuilderType TargetType => this.target?.FieldType;
    }
}