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

        public new BooleanExpressionResultCoder Is(BuilderType type) => this.Implement().Is(type);

        public new BooleanExpressionResultCoder Is(Type type) => this.Implement().Is(type);

        public new BooleanExpressionResultCoder IsFalse() => this.Implement().IsFalse();

        public new BooleanExpressionResultCoder IsNotNull() => this.Implement().IsNotNull();

        public new BooleanExpressionResultCoder IsNull() => this.Implement().IsNull();

        public new BooleanExpressionResultCoder IsTrue() => this.Implement().IsTrue();

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
            var result = this.AreEqualInternalWithoutJump(this.target.FieldType, field.FieldType, new BooleanExpressionParameter(this), new BooleanExpressionParameter(field, field.FieldType.typeReference, false, false));
            result.coder.instructions.Append(result.coder.processor.Create(isBrTrue ? OpCodes.Brtrue : OpCodes.Brfalse, result.jumpTarget));
            result.isBrTrue = isBrTrue;

            return result;
        }

        private BooleanExpressionResultCoder EqualsToInternal(object value, bool isBrTrue)
        {
            var result = this.AreEqualInternalWithoutJump(this.target.FieldType, value.GetType().ToBuilderType(), new BooleanExpressionParameter(this), new BooleanExpressionParameter(value, null, false, false));
            result.coder.instructions.Append(result.coder.processor.Create(isBrTrue ? OpCodes.Brtrue : OpCodes.Brfalse, result.jumpTarget));
            result.isBrTrue = isBrTrue;

            return result;
        }
    }
}