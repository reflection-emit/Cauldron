using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionCallCoder
    {
        public BooleanExpressionCallCoder Call(Method method, params object[] parameters)
        {
            var instance = this.coder.NewCoder().CallInternal(this.instance, this.calledMethod, OpCodes.Call, this.parameters).ToCodeBlock(this.castToType);
            return new BooleanExpressionCallCoder(this.coder, this.jumpTarget, instance, method, parameters);
        }
    }
}