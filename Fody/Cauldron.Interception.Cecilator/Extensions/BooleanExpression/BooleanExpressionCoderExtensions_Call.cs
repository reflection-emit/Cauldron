using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public static partial class BooleanExpressionCoderExtensions
    {
        public static BooleanExpressionCallCoder As(this BooleanExpressionCallCoder coder, BuilderType type)
        {
            coder.castToType = type;
            return coder;
        }

        public static BooleanExpressionCallCoder Call(this BooleanExpressionCallCoder coder, Method method, params object[] parameters)
        {
            var instance = coder.coder.NewCoder().CallInternal(coder.instance, coder.calledMethod, OpCodes.Call, coder.parameters).ToCodeBlock(coder.castToType);
            return new BooleanExpressionCallCoder(coder.coder, coder.jumpTarget, instance, method, parameters);
        }

        public static BooleanExpressionResultCoder EqualsTo(this BooleanExpressionCallCoder coder, object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return EqualsToInternal(coder, value, false);
        }

        public static BooleanExpressionResultCoder EqualsTo(this BooleanExpressionCallCoder coder, Func<BooleanExpressionCoder, BooleanExpressionCallCoder> call)
            => EqualsToInternal(coder, call, false);

        public static BooleanExpressionResultCoder EqualsTo(this BooleanExpressionCallCoder coder, Field field) => EqualsToInternal(coder, field, false);

        public static BooleanExpressionCoder Invert(this BooleanExpressionCallCoder coder)
        {
            coder.coder.CallInternal(coder.instance, coder.calledMethod, OpCodes.Call, coder.parameters);
            coder.coder.instructions.Append(coder.coder.processor.Create(OpCodes.Ldc_I4_0));
            coder.coder.instructions.Append(coder.coder.processor.Create(OpCodes.Ceq));
            return new BooleanExpressionCoder(coder);
        }

        public static BooleanExpressionResultCoder Is(this BooleanExpressionCallCoder coder, Type type)
        {
            coder.coder.CallInternal(coder.instance, coder.calledMethod, OpCodes.Call, coder.parameters);
            return new BooleanExpressionCoder(coder).Is(type);
        }

        public static BooleanExpressionResultCoder IsFalse(this BooleanExpressionCallCoder coder)
        {
            coder.coder.CallInternal(coder.instance, coder.calledMethod, OpCodes.Call, coder.parameters);
            return new BooleanExpressionCoder(coder).IsFalse();
        }

        public static BooleanExpressionResultCoder IsNotNull(this BooleanExpressionCallCoder coder)
        {
            coder.coder.CallInternal(coder.instance, coder.calledMethod, OpCodes.Call, coder.parameters);
            return new BooleanExpressionCoder(coder).IsNotNull();
        }

        public static BooleanExpressionResultCoder IsNull(this BooleanExpressionCallCoder coder)
        {
            coder.coder.CallInternal(coder.instance, coder.calledMethod, OpCodes.Call, coder.parameters);
            return new BooleanExpressionCoder(coder).IsNull();
        }

        public static BooleanExpressionResultCoder IsTrue(this BooleanExpressionCallCoder coder)
        {
            coder.coder.CallInternal(coder.instance, coder.calledMethod, OpCodes.Call, coder.parameters);
            return new BooleanExpressionCoder(coder).IsTrue();
        }

        public static BooleanExpressionCoder Negate(this BooleanExpressionCallCoder coder)
        {
            coder.coder.CallInternal(coder.instance, coder.calledMethod, OpCodes.Call, coder.parameters);
            coder.coder.instructions.Append(coder.coder.processor.Create(OpCodes.Neg));
            return new BooleanExpressionCoder(coder);
        }

        public static BooleanExpressionResultCoder NotEqualsTo(this BooleanExpressionCallCoder coder, Field field) => EqualsToInternal(coder, field, true);

        public static BooleanExpressionResultCoder NotEqualsTo(this BooleanExpressionCallCoder coder, object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return EqualsToInternal(coder, value, true);
        }

        private static BooleanExpressionResultCoder EqualsToInternal(
            BooleanExpressionCallCoder coder,
            Func<BooleanExpressionCoder, BooleanExpressionCallCoder> call,
            bool isBrTrue)
        {
            var method1 = coder.coder.NewCoder().CallInternal(coder.instance, coder.calledMethod, OpCodes.Call, coder.parameters).ToCodeBlock(coder.castToType);
            var otherCoder = new BooleanExpressionCoder(coder.coder.NewCoder());
            var callCoder = call(otherCoder);
            var method2 = callCoder.coder.NewCoder().CallInternal(callCoder.instance, callCoder.calledMethod, OpCodes.Call, callCoder.parameters).ToCodeBlock();

            var result = coder.AreEqualInternalWithoutJump(
                coder.calledMethod.ReturnType,
                callCoder.calledMethod.ReturnType,
                new BooleanExpressionParameter(method1, coder.calledMethod.ReturnType.typeReference, false, false),
                new BooleanExpressionParameter(method2, callCoder.calledMethod.ReturnType.typeReference, false, false));
            result.coder.instructions.Append(result.coder.processor.Create(isBrTrue ? OpCodes.Brtrue : OpCodes.Brfalse, result.jumpTarget));
            result.isBrTrue = isBrTrue;

            return result;
        }

        private static BooleanExpressionResultCoder EqualsToInternal(BooleanExpressionCallCoder coder, Field field, bool isBrTrue)
        {
            var method = coder.coder.NewCoder().CallInternal(coder.instance, coder.calledMethod, OpCodes.Call, coder.parameters).ToCodeBlock(coder.castToType);
            var result = coder.AreEqualInternalWithoutJump(coder.calledMethod.ReturnType, field.FieldType,
                new BooleanExpressionParameter(method, coder.calledMethod.ReturnType.typeReference, false, false),
                new BooleanExpressionParameter(field, field.FieldType.typeReference, false, false));
            result.coder.instructions.Append(result.coder.processor.Create(isBrTrue ? OpCodes.Brtrue : OpCodes.Brfalse, result.jumpTarget));
            result.isBrTrue = isBrTrue;

            return result;
        }

        private static BooleanExpressionResultCoder EqualsToInternal(BooleanExpressionCallCoder coder, object value, bool isBrTrue)
        {
            var method = coder.coder.NewCoder().CallInternal(coder.instance, coder.calledMethod, OpCodes.Call, coder.parameters).ToCodeBlock(coder.castToType);
            var result = coder.AreEqualInternalWithoutJump(coder.calledMethod.ReturnType, value.GetType().ToBuilderType(),
                new BooleanExpressionParameter(method, coder.calledMethod.ReturnType.typeReference, false, false),
                new BooleanExpressionParameter(value, null, false, false));
            result.coder.instructions.Append(result.coder.processor.Create(isBrTrue ? OpCodes.Brtrue : OpCodes.Brfalse, result.jumpTarget));
            result.isBrTrue = isBrTrue;

            return result;
        }
    }
}