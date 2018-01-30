using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionCoder : IRelationalOperators<BooleanExpressionResultCoder>
    {
        public BooleanExpressionResultCoder Is(BuilderType type)
        {
            var x = this.coder;
            var result = new BooleanExpressionResultCoder(x, this.jumpTarget);

            x.instructions.Append(x.processor.Create(OpCodes.Isinst, type.Import().typeReference));
            x.instructions.Append(x.processor.Create(OpCodes.Ldnull));
            x.instructions.Append(x.processor.Create(OpCodes.Cgt_Un));
            x.instructions.Append(x.processor.Create(OpCodes.Brfalse, result.jumpTarget));

            return result;
        }

        public BooleanExpressionResultCoder Is(Type type)
        {
            var x = this.coder;
            var result = new BooleanExpressionResultCoder(x, this.jumpTarget);

            x.instructions.Append(x.processor.Create(OpCodes.Isinst, Builder.Current.Import(type)));
            x.instructions.Append(x.processor.Create(OpCodes.Ldnull));
            x.instructions.Append(x.processor.Create(OpCodes.Cgt_Un));
            x.instructions.Append(x.processor.Create(OpCodes.Brfalse, result.jumpTarget));

            return result;
        }

        public BooleanExpressionResultCoder IsFalse()
        {
            var x = this.coder;
            var result = new BooleanExpressionResultCoder(x, this.jumpTarget, true);

            x.instructions.Append(x.processor.Create(OpCodes.Ldc_I4_1));
            x.instructions.Append(x.processor.Create(OpCodes.Ceq));
            x.instructions.Append(x.processor.Create(OpCodes.Brtrue, result.jumpTarget));

            return result;
        }

        public BooleanExpressionResultCoder IsNotNull()
        {
            var x = this.coder;
            var result = new BooleanExpressionResultCoder(x, this.jumpTarget, true);

            x.instructions.Append(x.processor.Create(OpCodes.Ldnull));
            x.instructions.Append(x.processor.Create(OpCodes.Ceq));
            x.instructions.Append(x.processor.Create(OpCodes.Brtrue, result.jumpTarget));

            return result;
        }

        public BooleanExpressionResultCoder IsNull()
        {
            var x = this.coder;
            var result = new BooleanExpressionResultCoder(x, this.jumpTarget);

            x.instructions.Append(x.processor.Create(OpCodes.Ldnull));
            x.instructions.Append(x.processor.Create(OpCodes.Ceq));
            x.instructions.Append(x.processor.Create(OpCodes.Brfalse, result.jumpTarget));

            return result;
        }

        public BooleanExpressionResultCoder IsTrue()
        {
            var x = this.coder;
            var result = new BooleanExpressionResultCoder(x, this.jumpTarget);

            x.instructions.Append(x.processor.Create(OpCodes.Ldc_I4_1));
            x.instructions.Append(x.processor.Create(OpCodes.Ceq));
            x.instructions.Append(x.processor.Create(OpCodes.Brfalse, result.jumpTarget));

            return result;
        }
    }
}