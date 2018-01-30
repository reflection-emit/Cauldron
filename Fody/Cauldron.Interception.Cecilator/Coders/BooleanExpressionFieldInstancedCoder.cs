using Mono.Cecil;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionFieldInstancedCoder : BooleanExpressionInstanceCoder<Field>
    {
        internal BooleanExpressionFieldInstancedCoder(BooleanExpressionCoder coder, Field target) :
            base(coder.coder, target)
        {
        }

        public override TypeReference TargetType => this.target?.fieldRef.FieldType;
    }
}