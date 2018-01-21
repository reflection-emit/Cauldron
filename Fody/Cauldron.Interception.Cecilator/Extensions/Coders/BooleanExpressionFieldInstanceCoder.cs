using Mono.Cecil;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public class BooleanExpressionFieldInstancCoder : BooleanExpressionInstanceCoder<Field>
    {
        internal BooleanExpressionFieldInstancCoder(BooleanExpressionCoder coder, Field target) :
            base(coder.coder, target)
        {
        }

        public override TypeReference TargetType => this.target?.fieldRef.FieldType;
    }
}