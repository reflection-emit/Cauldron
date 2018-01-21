using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public static class BooleanExpressionCoderExtensions
    {
        public static BooleanExpressionResultCoder And(this BooleanExpressionResultCoder coder, Func<BooleanExpressionCoder, BooleanExpressionResultCoder> other)
        {
            var x = coder.coder;
            var otherCoder = new BooleanExpressionCoder(coder.coder);
            other(otherCoder);
            x.instructions.Append(x.processor.Create(OpCodes.And));
            return coder;
        }

        public static BooleanExpressionResultCoder And(this BooleanExpressionResultCoder coder, Field field)
        {
            var x = coder.coder;
            var otherCoder = new BooleanExpressionCoder(coder.coder);
            x.instructions.Append(x.AddParameter(x.processor, Builder.Current.TypeSystem.Boolean, field).Instructions);
            x.instructions.Append(x.processor.Create(OpCodes.And));
            return coder;
        }

        public static BooleanExpressionResultCoder And(this BooleanExpressionResultCoder coder, LocalVariable variable)
        {
            var x = coder.coder;
            var otherCoder = new BooleanExpressionCoder(coder.coder);
            x.instructions.Append(x.AddParameter(x.processor, Builder.Current.TypeSystem.Boolean, variable).Instructions);
            x.instructions.Append(x.processor.Create(OpCodes.And));
            return coder;
        }

        /// <summary>
        /// Calls a instanced or static <see cref="Method"/> that exists in the loaded field.
        /// </summary>
        /// <param name="coder"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static BooleanExpressionCoder Call(this BooleanExpressionFieldInstancCoder coder, Method method, params object[] parameters)
        {
            coder.coder.CallInternal(coder.target, method, OpCodes.Call, parameters);
            return new BooleanExpressionCoder(coder.coder);
        }

        /// <summary>
        /// Calls a instanced or static <see cref="Method"/> that exists in the declaring type.
        /// </summary>
        /// <param name="coder"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static BooleanExpressionCoder Call(this BooleanExpressionCoder coder, Method method, params object[] parameters)
        {
            coder.coder.CallInternal(Crumb.This, method, OpCodes.Call, parameters);
            return new BooleanExpressionCoder(coder.coder);
        }

        public static BooleanExpressionResultCoder EqualsTo(this BooleanExpressionFieldInstancCoder coder, Field field)
        {
            var result = coder.AreEqualInternalWithoutJump(coder.target.FieldType, field.FieldType, coder.target, field);
            result.coder.instructions.Append(result.coder.processor.Create(OpCodes.Brfalse, result.jumpTarget));
            return result;
        }

        public static BooleanExpressionResultCoder Is(this BooleanExpressionCoder coder, BuilderType type)
        {
            var x = coder.coder;
            var result = new BooleanExpressionResultCoder(x);

            x.instructions.Append(x.processor.Create(OpCodes.Isinst, type.Import().typeReference));
            x.instructions.Append(x.processor.Create(OpCodes.Ldnull));
            x.instructions.Append(x.processor.Create(OpCodes.Cgt_Un));
            x.instructions.Append(x.processor.Create(OpCodes.Brfalse, result.jumpTarget));

            return result;
        }

        public static BooleanExpressionResultCoder Is(this BooleanExpressionCoder coder, Type type)
        {
            var x = coder.coder;
            var result = new BooleanExpressionResultCoder(x);

            x.instructions.Append(x.processor.Create(OpCodes.Isinst, Builder.Current.Import(type)));
            x.instructions.Append(x.processor.Create(OpCodes.Ldnull));
            x.instructions.Append(x.processor.Create(OpCodes.Cgt_Un));
            x.instructions.Append(x.processor.Create(OpCodes.Brfalse, result.jumpTarget));

            return result;
        }

        public static BooleanExpressionResultCoder IsFalse(this BooleanExpressionCoder coder)
        {
            var x = coder.coder;
            var result = new BooleanExpressionResultCoder(x);

            x.instructions.Append(x.processor.Create(OpCodes.Ldc_I4_1));
            x.instructions.Append(x.processor.Create(OpCodes.Ceq));
            x.instructions.Append(x.processor.Create(OpCodes.Brtrue, result.jumpTarget));

            return result;
        }

        public static BooleanExpressionResultCoder IsNotNull(this BooleanExpressionCoder coder)
        {
            var x = coder.coder;
            var result = new BooleanExpressionResultCoder(x);

            x.instructions.Append(x.processor.Create(OpCodes.Ldnull));
            x.instructions.Append(x.processor.Create(OpCodes.Ceq));
            x.instructions.Append(x.processor.Create(OpCodes.Brtrue, result.jumpTarget));

            return result;
        }

        public static BooleanExpressionResultCoder IsNull(this BooleanExpressionCoder coder)
        {
            var x = coder.coder;
            var result = new BooleanExpressionResultCoder(x);

            x.instructions.Append(x.processor.Create(OpCodes.Ldnull));
            x.instructions.Append(x.processor.Create(OpCodes.Ceq));
            x.instructions.Append(x.processor.Create(OpCodes.Brfalse, result.jumpTarget));

            return result;
        }

        public static BooleanExpressionResultCoder IsTrue(this BooleanExpressionCoder coder)
        {
            var x = coder.coder;
            var result = new BooleanExpressionResultCoder(x);

            x.instructions.Append(x.processor.Create(OpCodes.Ldc_I4_1));
            x.instructions.Append(x.processor.Create(OpCodes.Ceq));
            x.instructions.Append(x.processor.Create(OpCodes.Brfalse, result.jumpTarget));

            return result;
        }

        public static BooleanExpressionFieldInstancCoder Load(this BooleanExpressionCoder coder, Field value) => new BooleanExpressionFieldInstancCoder(coder, value);

        public static BooleanExpressionCoder Negate(this BooleanExpressionCoder coder)
        {
            coder.coder.instructions.Append(coder.coder.processor.Create(OpCodes.Neg));
            return coder;
        }

        public static BooleanExpressionResultCoder Negate(this BooleanExpressionResultCoder coder)
        {
            coder.coder.instructions.Append(coder.coder.processor.Create(OpCodes.Neg));
            return coder;
        }

        public static BooleanExpressionResultCoder NotEqualsTo(this BooleanExpressionFieldInstancCoder coder, Field field)
        {
            var result = coder.AreEqualInternalWithoutJump(coder.target.FieldType, field.FieldType, coder.target, field);
            result.coder.instructions.Append(result.coder.processor.Create(OpCodes.Brtrue, result.jumpTarget));
            return result;
        }

        public static BooleanExpressionResultCoder Or(this BooleanExpressionResultCoder coder, Field field)
        {
            var x = coder.coder;
            var otherCoder = new BooleanExpressionCoder(coder.coder);
            x.instructions.Append(x.AddParameter(x.processor, Builder.Current.TypeSystem.Boolean, field).Instructions);
            x.instructions.Append(x.processor.Create(OpCodes.Or));
            return coder;
        }

        public static BooleanExpressionResultCoder Or(this BooleanExpressionResultCoder coder, LocalVariable variable)
        {
            var x = coder.coder;
            var otherCoder = new BooleanExpressionCoder(coder.coder);
            x.instructions.Append(x.AddParameter(x.processor, Builder.Current.TypeSystem.Boolean, variable).Instructions);
            x.instructions.Append(x.processor.Create(OpCodes.Or));
            return coder;
        }

        public static BooleanExpressionResultCoder Or(this BooleanExpressionResultCoder coder, Func<BooleanExpressionCoder, BooleanExpressionResultCoder> other)
        {
            var x = coder.coder;
            var otherCoder = new BooleanExpressionCoder(coder.coder);
            other(otherCoder);
            x.instructions.Append(x.processor.Create(OpCodes.Or));
            return coder;
        }

        private static BooleanExpressionResultCoder AreEqualInternalWithoutJump(this BooleanExpressionCoder coder, BuilderType a, BuilderType b, object valueA, object valueB)
        {
            // TODO - needs to handle Nullables

            var x = coder.coder;
            var result = new BooleanExpressionResultCoder(x);

            if (a == b && a.IsPrimitive)
            {
                x.instructions.Append(x.AddParameter(x.processor, a.typeReference, valueA).Instructions);
                x.instructions.Append(x.AddParameter(x.processor, b.typeReference, valueB).Instructions);
                x.instructions.Append(x.processor.Create(OpCodes.Ceq));

                return result;
            }

            var equalityOperator = a.GetMethod("op_Equality", false, a, b)?.Import();

            if (equalityOperator != null)
            {
                coder.coder.Call(equalityOperator.Import(), valueA, valueB);
                return result;
            }

            equalityOperator = b.GetMethod("op_Equality", false, b, a)?.Import();

            if (equalityOperator != null)
            {
                coder.coder.Call(equalityOperator.Import(), valueB, valueA);
                return result;
            }

            equalityOperator = Builder.Current.GetType(typeof(object)).GetMethod("Equals", false, "System.Object", "System.Object").Import();

            coder.coder.Call(equalityOperator.Import(), valueA, valueB);
            return result;
        }
    }
}