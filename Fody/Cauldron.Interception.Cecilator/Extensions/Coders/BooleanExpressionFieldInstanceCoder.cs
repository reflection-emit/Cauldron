using Mono.Cecil;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public class BooleanExpressionFieldInstancedCoder : BooleanExpressionInstanceCoder<Field>
    {
        internal BooleanExpressionFieldInstancedCoder(BooleanExpressionCoder coder, Field target) :
            base(coder.coder, target)
        {
        }

        public override TypeReference TargetType => this.target?.fieldRef.FieldType;
    }
}