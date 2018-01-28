using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public static partial class BooleanExpressionCoderExtensions
    {
        public static BooleanExpressionResultCoder And(this BooleanExpressionResultCoder coder, Func<BooleanExpressionCoder, BooleanExpressionResultCoder> other)
        {
            var x = coder.coder;
            var otherCoder = new BooleanExpressionCoder(coder.coder);
            coder.RemoveJump();
            other(otherCoder);
            x.instructions.Append(x.processor.Create(OpCodes.And));
            return coder;
        }

        public static BooleanExpressionResultCoder And(this BooleanExpressionResultCoder coder, Field field)
        {
            var x = coder.coder;
            var otherCoder = new BooleanExpressionCoder(coder.coder);
            coder.RemoveJump();
            x.instructions.Append(x.AddParameter(x.processor, Builder.Current.TypeSystem.Boolean, field).Instructions);
            x.instructions.Append(x.processor.Create(OpCodes.And));
            return coder;
        }

        public static BooleanExpressionResultCoder And(this BooleanExpressionResultCoder coder, LocalVariable variable)
        {
            var x = coder.coder;
            var otherCoder = new BooleanExpressionCoder(coder.coder);
            coder.RemoveJump();
            x.instructions.Append(x.AddParameter(x.processor, Builder.Current.TypeSystem.Boolean, variable).Instructions);
            x.instructions.Append(x.processor.Create(OpCodes.And));
            return coder;
        }

        public static BooleanExpressionCoder AndAnd(this BooleanExpressionResultCoder coder) => new BooleanExpressionCoder(coder.coder, coder.jumpTarget);

        /// <summary>
        /// Calls a instanced or static <see cref="Method"/> that exists in the declaring type.
        /// </summary>
        /// <param name="coder"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static BooleanExpressionCallCoder Call(this BooleanExpressionCoder coder, Method method, params object[] parameters)
        {
            if (method.ReturnType.typeDefinition == Builder.Current.TypeSystem.Void)
                throw new InvalidOperationException("Void method are not supported by this call.");

            return new BooleanExpressionCallCoder(coder.coder, coder.jumpTarget, CodeBlocks.This, method, parameters);
        }

        public static BooleanExpressionResultCoder Is(this BooleanExpressionCoder coder, BuilderType type)
        {
            var x = coder.coder;
            var result = new BooleanExpressionResultCoder(x, coder.jumpTarget);

            x.instructions.Append(x.processor.Create(OpCodes.Isinst, type.Import().typeReference));
            x.instructions.Append(x.processor.Create(OpCodes.Ldnull));
            x.instructions.Append(x.processor.Create(OpCodes.Cgt_Un));
            x.instructions.Append(x.processor.Create(OpCodes.Brfalse, result.jumpTarget));

            return result;
        }

        public static BooleanExpressionResultCoder Is(this BooleanExpressionCoder coder, Type type)
        {
            var x = coder.coder;
            var result = new BooleanExpressionResultCoder(x, coder.jumpTarget);

            x.instructions.Append(x.processor.Create(OpCodes.Isinst, Builder.Current.Import(type)));
            x.instructions.Append(x.processor.Create(OpCodes.Ldnull));
            x.instructions.Append(x.processor.Create(OpCodes.Cgt_Un));
            x.instructions.Append(x.processor.Create(OpCodes.Brfalse, result.jumpTarget));

            return result;
        }

        public static BooleanExpressionResultCoder IsFalse(this BooleanExpressionCoder coder)
        {
            var x = coder.coder;
            var result = new BooleanExpressionResultCoder(x, coder.jumpTarget, true);

            x.instructions.Append(x.processor.Create(OpCodes.Ldc_I4_1));
            x.instructions.Append(x.processor.Create(OpCodes.Ceq));
            x.instructions.Append(x.processor.Create(OpCodes.Brtrue, result.jumpTarget));

            return result;
        }

        public static BooleanExpressionResultCoder IsNotNull(this BooleanExpressionCoder coder)
        {
            var x = coder.coder;
            var result = new BooleanExpressionResultCoder(x, coder.jumpTarget, true);

            x.instructions.Append(x.processor.Create(OpCodes.Ldnull));
            x.instructions.Append(x.processor.Create(OpCodes.Ceq));
            x.instructions.Append(x.processor.Create(OpCodes.Brtrue, result.jumpTarget));

            return result;
        }

        public static BooleanExpressionResultCoder IsNull(this BooleanExpressionCoder coder)
        {
            var x = coder.coder;
            var result = new BooleanExpressionResultCoder(x, coder.jumpTarget);

            x.instructions.Append(x.processor.Create(OpCodes.Ldnull));
            x.instructions.Append(x.processor.Create(OpCodes.Ceq));
            x.instructions.Append(x.processor.Create(OpCodes.Brfalse, result.jumpTarget));

            return result;
        }

        public static BooleanExpressionResultCoder IsTrue(this BooleanExpressionCoder coder)
        {
            var x = coder.coder;
            var result = new BooleanExpressionResultCoder(x, coder.jumpTarget);

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

        public static BooleanExpressionResultCoder Or(this BooleanExpressionResultCoder coder, Field field)
        {
            var x = coder.coder;
            var otherCoder = new BooleanExpressionCoder(coder.coder);
            coder.RemoveJump();
            x.instructions.Append(x.AddParameter(x.processor, Builder.Current.TypeSystem.Boolean, field).Instructions);
            x.instructions.Append(x.processor.Create(OpCodes.Or));
            return coder;
        }

        public static BooleanExpressionResultCoder Or(this BooleanExpressionResultCoder coder, LocalVariable variable)
        {
            var x = coder.coder;
            var otherCoder = new BooleanExpressionCoder(coder.coder);
            coder.RemoveJump();
            x.instructions.Append(x.AddParameter(x.processor, Builder.Current.TypeSystem.Boolean, variable).Instructions);
            x.instructions.Append(x.processor.Create(OpCodes.Or));
            return coder;
        }

        public static BooleanExpressionResultCoder Or(this BooleanExpressionResultCoder coder, Func<BooleanExpressionCoder, BooleanExpressionResultCoder> other)
        {
            var x = coder.coder;
            var otherCoder = new BooleanExpressionCoder(coder.coder);
            coder.RemoveJump();
            other(otherCoder);
            x.instructions.Append(x.processor.Create(OpCodes.Or));
            return coder;
        }

        private static BooleanExpressionResultCoder AreEqualInternalWithoutJump(this ContextCoder coder, BuilderType a, BuilderType b, object valueA, object valueB)
        {
            // TODO - needs to handle Nullables

            var x = coder.coder;
            var result = new BooleanExpressionResultCoder(x, coder.jumpTarget);

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

            // This is a special case for stuff we surely know how to convert
            // It is almost like the first block, but with forced target types
            if (a.IsPrimitive && b.IsPrimitive)
            {
                var typeToUse = GetTypeWithMoreCapacity(a.typeReference, b.typeReference);
                x.instructions.Append(x.AddParameter(x.processor, typeToUse, valueA).Instructions);
                x.instructions.Append(x.AddParameter(x.processor, typeToUse, valueB).Instructions);
                x.instructions.Append(x.processor.Create(OpCodes.Ceq));

                return result;
            }

            equalityOperator = Builder.Current.GetType(typeof(object)).GetMethod("Equals", false, "System.Object", "System.Object").Import();

            coder.coder.Call(equalityOperator.Import(), valueA, valueB);
            return result;
        }

        private static TypeReference GetTypeWithMoreCapacity(TypeReference a, TypeReference b)
        {
            // Bool makes a big exception here
            if (a.FullName == typeof(bool).FullName) return a;
            if (b.FullName == typeof(bool).FullName) return b;

            if (a.FullName == typeof(decimal).FullName) return a;
            if (b.FullName == typeof(decimal).FullName) return b;

            if (a.FullName == typeof(double).FullName) return a;
            if (b.FullName == typeof(double).FullName) return b;

            if (a.FullName == typeof(float).FullName) return a;
            if (b.FullName == typeof(float).FullName) return b;

            if (a.FullName == typeof(ulong).FullName) return a;
            if (b.FullName == typeof(ulong).FullName) return b;

            if (a.FullName == typeof(long).FullName) return a;
            if (b.FullName == typeof(long).FullName) return b;

            if (a.FullName == typeof(uint).FullName) return a;
            if (b.FullName == typeof(uint).FullName) return b;

            if (a.FullName == typeof(int).FullName) return a;
            if (b.FullName == typeof(int).FullName) return b;

            if (a.FullName == typeof(char).FullName) return a;
            if (b.FullName == typeof(char).FullName) return b;

            if (a.FullName == typeof(ushort).FullName) return a;
            if (b.FullName == typeof(ushort).FullName) return b;

            if (a.FullName == typeof(short).FullName) return a;
            if (b.FullName == typeof(short).FullName) return b;

            if (a.FullName == typeof(byte).FullName) return a;
            if (b.FullName == typeof(byte).FullName) return b;

            if (a.FullName == typeof(sbyte).FullName) return a;
            if (b.FullName == typeof(sbyte).FullName) return b;

            return a;
        }
    }
}