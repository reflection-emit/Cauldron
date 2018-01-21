using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public sealed class Coder
    {
        public const string ReturnVariableName = "<>returnValue";
        public const string VariablePrefix = "<>var_";
        internal readonly InstructionContainer instructions = new InstructionContainer();
        internal readonly Method method;
        internal readonly ILProcessor processor;

        internal Coder(Method method)
        {
            this.method = method;
            this.processor = method.GetILProcessor();
            this.method.methodDefinition.Body.SimplifyMacros();
        }

        internal Coder(Method method, InstructionContainer instructions) : this(method) => this.instructions.Append(instructions);

        /// <summary>
        /// Generates a random name that can be used to name variables and methods.
        /// </summary>
        /// <returns></returns>
        public static string GenerateName() => Path.GetRandomFileName().Replace(".", DateTime.Now.Second.ToString());

        public static bool operator !=(Coder a, Coder b) => !(a == b);

        public static bool operator ==(Coder a, Coder b)
        {
            if (object.Equals(a, null) && object.Equals(b, null))
                return true;

            if (object.Equals(a, null))
                return false;

            return a.Equals(b);
        }

        public Coder Append(IEnumerable<Instruction> instructions)
        {
            this.instructions.Append(instructions);
            return this;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null: return false;
                case Coder coder:
                    if (this.GetHashCode() != coder.GetHashCode())
                        return false;

                    return this.instructions.Count == coder.instructions.Count && this.method == coder.method;

                default: return false;
            }
        }

        public override int GetHashCode() => this.GetType().GetHashCode() ^ this.method.GetHashCode() ^ this.instructions.GetHashCode();

        public void InstructionDebug() => this.method.Log(LogTypes.Info, this.instructions);

        public CodeSet ToCodeSet() => new CoderCodeSet(this);

        public override string ToString() => this.method.Fullname;
    }
}