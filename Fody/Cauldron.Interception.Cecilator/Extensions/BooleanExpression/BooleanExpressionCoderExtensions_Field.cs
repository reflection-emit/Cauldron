using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public static partial class BooleanExpressionCoderExtensions
    {
        /// <summary>
        /// Calls a instanced or static <see cref="Method"/> that exists in the loaded field.
        /// </summary>
        /// <param name="coder"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static BooleanExpressionCallCoder Call(this BooleanExpressionFieldInstancedCoder coder, Method method, params object[] parameters)
        {
            coder.coder.CallInternal(coder.target, method, OpCodes.Call, parameters);
            return new BooleanExpressionCallCoder(coder.coder, coder.jumpTarget, coder.target, method, parameters);
        }

        public static BooleanExpressionResultCoder EqualsTo(this BooleanExpressionFieldInstancedCoder coder, object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return EqualsToInternal(coder, value, false);
        }

        public static BooleanExpressionResultCoder EqualsTo(this BooleanExpressionFieldInstancedCoder coder, Field field)
            => EqualsToInternal(coder, field, false);

        public static BooleanExpressionFieldInstancedCoder Invert(this BooleanExpressionFieldInstancedCoder coder)
        {
            coder.invert = true;
            return coder;
        }

        public static BooleanExpressionResultCoder Is(this BooleanExpressionFieldInstancedCoder coder, BuilderType type) => coder.Implement().Is(type);

        public static BooleanExpressionResultCoder Is(this BooleanExpressionFieldInstancedCoder coder, Type type) => coder.Implement().Is(type);

        public static BooleanExpressionResultCoder IsFalse(this BooleanExpressionFieldInstancedCoder coder) => coder.Implement().IsFalse();

        public static BooleanExpressionResultCoder IsNotNull(this BooleanExpressionFieldInstancedCoder coder) => coder.Implement().IsNotNull();

        public static BooleanExpressionResultCoder IsNull(this BooleanExpressionFieldInstancedCoder coder) => coder.Implement().IsNull();

        public static BooleanExpressionResultCoder IsTrue(this BooleanExpressionFieldInstancedCoder coder) => coder.Implement().IsTrue();

        public static BooleanExpressionFieldInstancedCoder Negate(this BooleanExpressionFieldInstancedCoder coder)
        {
            coder.negate = true;
            return coder;
        }

        public static BooleanExpressionResultCoder NotEqualsTo(this BooleanExpressionFieldInstancedCoder coder, object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return EqualsToInternal(coder, value, true);
        }

        public static BooleanExpressionResultCoder NotEqualsTo(this BooleanExpressionFieldInstancedCoder coder, Field field)
            => EqualsToInternal(coder, field, true);

        private static BooleanExpressionResultCoder EqualsToInternal(BooleanExpressionFieldInstancedCoder coder, Field field, bool isBrTrue)
        {
            var result = coder.AreEqualInternalWithoutJump(coder.target.FieldType, field.FieldType, new BooleanExpressionParameter(coder), new BooleanExpressionParameter(field, field.FieldType.typeReference, false, false));
            result.coder.instructions.Append(result.coder.processor.Create(isBrTrue ? OpCodes.Brtrue : OpCodes.Brfalse, result.jumpTarget));
            result.isBrTrue = isBrTrue;

            return result;
        }

        private static BooleanExpressionResultCoder EqualsToInternal(BooleanExpressionFieldInstancedCoder coder, object value, bool isBrTrue)
        {
            var result = coder.AreEqualInternalWithoutJump(coder.target.FieldType, value.GetType().ToBuilderType(), new BooleanExpressionParameter(coder), new BooleanExpressionParameter(value, null, false, false));
            result.coder.instructions.Append(result.coder.processor.Create(isBrTrue ? OpCodes.Brtrue : OpCodes.Brfalse, result.jumpTarget));
            result.isBrTrue = isBrTrue;

            return result;
        }
    }
}