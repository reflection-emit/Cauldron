using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Coders
{
    public abstract class BooleanExpressionContextCoder : ContextCoder
    {
        internal readonly Instruction jumpTarget;

        internal BooleanExpressionContextCoder(BooleanExpressionContextCoder coder) :
            base(coder.coder, true) => this.jumpTarget = coder.jumpTarget;

        internal BooleanExpressionContextCoder(BooleanExpressionContextCoder coder, bool autoAddThisInstance) :
            base(coder.coder, autoAddThisInstance) => this.jumpTarget = coder.jumpTarget;

        internal BooleanExpressionContextCoder(Coder coder) :
            base(coder, true) => this.jumpTarget = coder.instructions.ilprocessor.Create(OpCodes.Nop);

        internal BooleanExpressionContextCoder(Coder coder, bool autoAddThisInstance) :
            base(coder, autoAddThisInstance) => this.jumpTarget = coder.instructions.ilprocessor.Create(OpCodes.Nop);

        internal BooleanExpressionContextCoder(Coder coder, Instruction jumpTarget) :
            base(coder, true) => this.jumpTarget = jumpTarget;

        internal BooleanExpressionContextCoder(Coder coder, Instruction jumpTarget, bool autoAddThisInstance) :
            base(coder, autoAddThisInstance) => this.jumpTarget = jumpTarget;

        public void InstructionDebug() => this.coder.InstructionDebug();

        internal BooleanExpressionResultCoder AreEqualInternalWithoutJump(BuilderType a, BuilderType b, BooleanExpressionParameter valueA, BooleanExpressionParameter valueB)
        {
            // TODO - needs to handle Nullables

            var x = this.coder;
            var result = new BooleanExpressionResultCoder(x, this.jumpTarget);

            if (a == b && a.IsPrimitive)
            {
                x.instructions.Append(InstructionBlock.CreateCode(x.instructions, a.typeReference, valueA));
                x.instructions.Append(InstructionBlock.CreateCode(x.instructions, b.typeReference, valueB));
                x.instructions.Emit(OpCodes.Ceq);

                return result;
            }

            var equalityOperator = a.GetMethod("op_Equality", false, a, b)?.Import();

            if (equalityOperator != null)
            {
                this.coder.Call(equalityOperator.Import(), valueA, valueB);
                return result;
            }

            equalityOperator = b.GetMethod("op_Equality", false, b, a)?.Import();

            if (equalityOperator != null)
            {
                this.coder.Call(equalityOperator.Import(), valueB, valueA);
                return result;
            }

            // This is a special case for stuff we surely know how to convert
            // It is almost like the first block, but with forced target types
            if (a.IsPrimitive && b.IsPrimitive)
            {
                var typeToUse = GetTypeWithMoreCapacity(a.typeReference, b.typeReference);
                x.instructions.Append(InstructionBlock.CreateCode(x.instructions, typeToUse, valueA));
                x.instructions.Append(InstructionBlock.CreateCode(x.instructions, typeToUse, valueB));
                x.instructions.Emit(OpCodes.Ceq);

                return result;
            }

            equalityOperator = Builder.Current.GetType(typeof(object)).GetMethod("Equals", false, "System.Object", "System.Object").Import();

            this.coder.Call(equalityOperator.Import(), valueA, valueB);
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