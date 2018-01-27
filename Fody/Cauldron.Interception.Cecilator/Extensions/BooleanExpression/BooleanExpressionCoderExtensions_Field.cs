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
        public static BooleanExpressionCallCoder Call(this BooleanExpressionFieldInstancCoder coder, Method method, params object[] parameters)
        {
            coder.coder.CallInternal(coder.target, method, OpCodes.Call, parameters);
            return new BooleanExpressionCallCoder(coder.coder, coder.jumpTarget, coder.target, method, parameters);
        }

        public static BooleanExpressionResultCoder EqualsTo(this BooleanExpressionFieldInstancCoder coder, Field field)
        {
            var result = coder.AreEqualInternalWithoutJump(coder.target.FieldType, field.FieldType, coder.target, field);
            result.coder.instructions.Append(result.coder.processor.Create(OpCodes.Brfalse, result.jumpTarget));
            return result;
        }

        public static BooleanExpressionResultCoder NotEqualsTo(this BooleanExpressionFieldInstancCoder coder, Field field)
        {
            var result = coder.AreEqualInternalWithoutJump(coder.target.FieldType, field.FieldType, coder.target, field);
            result.coder.instructions.Append(result.coder.processor.Create(OpCodes.Brtrue, result.jumpTarget));
            return new BooleanExpressionResultCoder(result.coder, coder.jumpTarget, true);
        }
    }
}