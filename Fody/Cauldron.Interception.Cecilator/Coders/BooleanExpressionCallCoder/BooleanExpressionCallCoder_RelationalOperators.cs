using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionCallCoder : IRelationalOperators<BooleanExpressionResultCoder, BooleanExpressionCoder, BooleanExpressionCallCoder>
    {
        public BooleanExpressionResultCoder EqualsTo(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return EqualsToInternal(this, value, false);
        }

        public BooleanExpressionResultCoder EqualsTo(Func<BooleanExpressionCoder, BooleanExpressionCallCoder> call)
            => EqualsToInternal(this, call, false);

        public BooleanExpressionResultCoder EqualsTo(Field field) => EqualsToInternal(this, field, false);

        public BooleanExpressionResultCoder Is(Type type)
        {
            this.coder.CallInternal(this.instance, this.calledMethod, OpCodes.Call, this.parameters);
            this.coder.Append(this.ImplementOperations());
            return new BooleanExpressionCoder(coder).Is(type);
        }

        public BooleanExpressionResultCoder Is(BuilderType type)
        {
            this.coder.CallInternal(this.instance, this.calledMethod, OpCodes.Call, this.parameters);
            this.coder.Append(this.ImplementOperations());
            return new BooleanExpressionCoder(coder).Is(type);
        }

        public BooleanExpressionResultCoder IsFalse()
        {
            this.coder.CallInternal(this.instance, this.calledMethod, OpCodes.Call, this.parameters);
            this.coder.Append(this.ImplementOperations());
            return new BooleanExpressionCoder(coder).IsFalse();
        }

        public BooleanExpressionResultCoder IsNotNull()
        {
            this.coder.CallInternal(this.instance, this.calledMethod, OpCodes.Call, this.parameters);
            this.coder.Append(this.ImplementOperations());
            return new BooleanExpressionCoder(coder).IsNotNull();
        }

        public BooleanExpressionResultCoder IsNull()
        {
            this.coder.CallInternal(this.instance, this.calledMethod, OpCodes.Call, this.parameters);
            this.coder.Append(this.ImplementOperations());
            return new BooleanExpressionCoder(coder).IsNull();
        }

        public BooleanExpressionResultCoder IsTrue()
        {
            this.coder.CallInternal(this.instance, this.calledMethod, OpCodes.Call, this.parameters);
            this.coder.Append(this.ImplementOperations());
            return new BooleanExpressionCoder(coder).IsTrue();
        }

        public BooleanExpressionResultCoder NotEqualsTo(Field field) => EqualsToInternal(this, field, true);

        public BooleanExpressionResultCoder NotEqualsTo(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return EqualsToInternal(this, value, true);
        }

        internal static BooleanExpressionResultCoder EqualsToInternal(
            BooleanExpressionCallCoder coder,
            Func<BooleanExpressionCoder, BooleanExpressionCallCoder> call,
            bool isBrTrue)
        {
            var method1 = coder.ToCodeBlock();
            var otherCoder = new BooleanExpressionCoder(coder.coder.NewCoder());
            var callCoder = call(otherCoder);
            var method2 = callCoder.coder.NewCoder().CallInternal(callCoder.instance, callCoder.calledMethod, OpCodes.Call, callCoder.parameters).ToCodeBlock();

            var result = coder.AreEqualInternalWithoutJump(
                coder.calledMethod.ReturnType,
                callCoder.calledMethod.ReturnType,
                new BooleanExpressionParameter(coder, method1, coder.calledMethod.ReturnType.typeReference),
                new BooleanExpressionParameter(coder, method2, callCoder.calledMethod.ReturnType.typeReference));
            result.coder.instructions.Append(result.coder.processor.Create(isBrTrue ? OpCodes.Brtrue : OpCodes.Brfalse, result.jumpTarget));
            result.isBrTrue = isBrTrue;

            return result;
        }

        internal static BooleanExpressionResultCoder EqualsToInternal(BooleanExpressionCallCoder coder, Field field, bool isBrTrue)
        {
            var method = coder.ToCodeBlock();
            var result = coder.AreEqualInternalWithoutJump(coder.calledMethod.ReturnType, field.FieldType,
                new BooleanExpressionParameter(coder, method, coder.calledMethod.ReturnType.typeReference),
                new BooleanExpressionParameter(coder, field, field.FieldType.typeReference));
            result.coder.instructions.Append(result.coder.processor.Create(isBrTrue ? OpCodes.Brtrue : OpCodes.Brfalse, result.jumpTarget));
            result.isBrTrue = isBrTrue;

            return result;
        }

        internal static BooleanExpressionResultCoder EqualsToInternal(BooleanExpressionCallCoder coder, object value, bool isBrTrue)
        {
            var method = coder.ToCodeBlock();
            var result = coder.AreEqualInternalWithoutJump(coder.calledMethod.ReturnType, value.GetType().ToBuilderType(),
                new BooleanExpressionParameter(coder, method, coder.calledMethod.ReturnType.typeReference),
                new BooleanExpressionParameter(coder, value, null));
            result.coder.instructions.Append(result.coder.processor.Create(isBrTrue ? OpCodes.Brtrue : OpCodes.Brfalse, result.jumpTarget));
            result.isBrTrue = isBrTrue;

            return result;
        }
    }
}