using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionFieldInstancedCoder : IRelationalOperators<BooleanExpressionResultCoder, BooleanExpressionCoder, BooleanExpressionFieldInstancedCoder>
    {
        public BooleanExpressionResultCoder EqualsTo(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return EqualsToInternal(value, false);
        }

        public BooleanExpressionResultCoder EqualsTo(Field field)
            => EqualsToInternal(field, false);

        public BooleanExpressionResultCoder EqualsTo(Func<BooleanExpressionCoder, BooleanExpressionFieldInstancedCoder> call)
        {
            throw new NotImplementedException();
        }

        public new BooleanExpressionResultCoder Is(BuilderType type) => this.ToBooleanExpressionCoder().Is(type);

        public new BooleanExpressionResultCoder Is(Type type) => this.ToBooleanExpressionCoder().Is(type);

        public new BooleanExpressionResultCoder IsFalse() => this.ToBooleanExpressionCoder().IsFalse();

        public new BooleanExpressionResultCoder IsNotNull() => this.ToBooleanExpressionCoder().IsNotNull();

        public new BooleanExpressionResultCoder IsNull() => this.ToBooleanExpressionCoder().IsNull();

        public new BooleanExpressionResultCoder IsTrue() => this.ToBooleanExpressionCoder().IsTrue();

        public BooleanExpressionResultCoder NotEqualsTo(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return EqualsToInternal(value, true);
        }

        public BooleanExpressionResultCoder NotEqualsTo(Field field)
            => EqualsToInternal(field, true);

        private BooleanExpressionResultCoder EqualsToInternal(Field field, bool isBrTrue)
        {
            var result = this.AreEqualInternalWithoutJump(this.target.FieldType, field.FieldType, new BooleanExpressionParameter(this), new BooleanExpressionParameter(this, field, field.FieldType.typeReference));
            result.coder.instructions.Append(result.coder.processor.Create(isBrTrue ? OpCodes.Brtrue : OpCodes.Brfalse, result.jumpTarget));
            result.isBrTrue = isBrTrue;

            return result;
        }

        private BooleanExpressionResultCoder EqualsToInternal(object value, bool isBrTrue)
        {
            var result = this.AreEqualInternalWithoutJump(this.target.FieldType, value.GetType().ToBuilderType(), new BooleanExpressionParameter(this), new BooleanExpressionParameter(this, value, null));
            result.coder.instructions.Append(result.coder.processor.Create(isBrTrue ? OpCodes.Brtrue : OpCodes.Brfalse, result.jumpTarget));
            result.isBrTrue = isBrTrue;

            return result;
        }
    }
}