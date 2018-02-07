using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class Coder
    {
        public Coder If(
            Func<BooleanExpressionCoder, BooleanExpressionResultCoder> booleanExpression,
            Action<Coder> then)
        {
            var result = booleanExpression(new BooleanExpressionCoder(this.NewCoder(), true));
            this.instructions.Append(result.coder.instructions);
            then(this);
            this.instructions.Append(result.jumpTarget);

            return this;
        }

        public Coder If(
            Func<BooleanExpressionCoder, BooleanExpressionResultCoder> booleanExpression,
            Action<Coder> then,
            Action<Coder> @else)
        {
            var result = booleanExpression(new BooleanExpressionCoder(this.NewCoder(), true));
            var endOfIf = this.instructions.ilprocessor.Create(OpCodes.Nop);
            this.instructions.Append(result.coder.instructions);
            then(this);
            this.instructions.Append(this.instructions.ilprocessor.Create(OpCodes.Br, endOfIf));
            this.instructions.Append(result.jumpTarget);
            @else(this);
            this.instructions.Append(endOfIf);

            return this;
        }
    }
}