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

            x.instructions.Emit(OpCodes.Isinst, type.Import().typeReference);
            x.instructions.Emit(OpCodes.Ldnull);
            x.instructions.Emit(OpCodes.Cgt_Un);
            x.instructions.Emit(OpCodes.Brfalse, result.jumpTarget);

            return result;
        }

        public BooleanExpressionResultCoder Is(Type type)
        {
            var x = this.coder;
            var result = new BooleanExpressionResultCoder(x, this.jumpTarget);

            x.instructions.Emit(OpCodes.Isinst, Builder.Current.Import(type));
            x.instructions.Emit(OpCodes.Ldnull);
            x.instructions.Emit(OpCodes.Cgt_Un);
            x.instructions.Emit(OpCodes.Brfalse, result.jumpTarget);

            return result;
        }

        public BooleanExpressionResultCoder IsFalse()
        {
            var x = this.coder;
            var result = new BooleanExpressionResultCoder(x, this.jumpTarget, true);

            x.instructions.Emit(OpCodes.Ldc_I4_1);
            x.instructions.Emit(OpCodes.Ceq);
            x.instructions.Emit(OpCodes.Brtrue, result.jumpTarget);

            return result;
        }

        public BooleanExpressionResultCoder IsNotNull()
        {
            var x = this.coder;
            var result = new BooleanExpressionResultCoder(x, this.jumpTarget, true);

            x.instructions.Emit(OpCodes.Ldnull);
            x.instructions.Emit(OpCodes.Ceq);
            x.instructions.Emit(OpCodes.Brtrue, result.jumpTarget);

            return result;
        }

        public BooleanExpressionResultCoder IsNull()
        {
            var x = this.coder;
            var result = new BooleanExpressionResultCoder(x, this.jumpTarget);

            x.instructions.Emit(OpCodes.Ldnull);
            x.instructions.Emit(OpCodes.Ceq);
            x.instructions.Emit(OpCodes.Brfalse, result.jumpTarget);

            return result;
        }

        public BooleanExpressionResultCoder IsTrue()
        {
            var x = this.coder;
            var result = new BooleanExpressionResultCoder(x, this.jumpTarget);

            x.instructions.Emit(OpCodes.Ldc_I4_1);
            x.instructions.Emit(OpCodes.Ceq);
            x.instructions.Emit(OpCodes.Brfalse, result.jumpTarget);

            return result;
        }
    }
}