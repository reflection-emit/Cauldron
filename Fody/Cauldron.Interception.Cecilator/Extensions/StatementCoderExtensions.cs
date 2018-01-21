using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public static class StatementCoderExtensions
    {
        public static Coder If(this Coder coder,
            Func<BooleanExpressionCoder, BooleanExpressionResultCoder> booleanExpression,
            Action<Coder> then)
        {
            var result = booleanExpression(new BooleanExpressionCoder(coder.NewCoder()));
            coder.instructions.Append(result.coder.instructions);
            then(coder);
            coder.instructions.Append(result.jumpTarget);

            return coder;
        }

        public static Coder If(this Coder coder,
            Func<BooleanExpressionCoder, BooleanExpressionResultCoder> booleanExpression,
            Action<Coder> then,
            Action<Coder> @else)
        {
            var result = booleanExpression(new BooleanExpressionCoder(coder.NewCoder()));
            var endOfIf = coder.processor.Create(OpCodes.Nop);
            coder.instructions.Append(result.coder.instructions);
            then(coder);
            coder.instructions.Append(coder.processor.Create(OpCodes.Br, endOfIf));
            coder.instructions.Append(result.jumpTarget);
            @else(coder);
            coder.instructions.Append(endOfIf);

            return coder;
        }
    }
}