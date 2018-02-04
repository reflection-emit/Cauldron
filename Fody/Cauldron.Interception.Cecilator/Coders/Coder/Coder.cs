using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed partial class Coder
    {
        public const string ReturnVariableName = "<>returnValue";
        public const string VariablePrefix = "<>var_";
        internal readonly InstructionBlock instructions;

        internal Coder(Method method)
        {
            instructions = (InstructionBlock)method;
        }

        internal Instruction LastInstruction => this.instructions.instructions.LastOrDefault();

        /// <summary>
        /// Generates a random name that can be used to name variables and methods.
        /// </summary>
        /// <returns></returns>
        public static string GenerateName() => Path.GetRandomFileName().Replace(".", DateTime.Now.Second.ToString());

        public static implicit operator InstructionBlock(Coder coder) => coder.instructions;

        public Coder Append(IEnumerable<Instruction> instructions)
        {
            this.instructions.Append(instructions);
            return this;
        }

        public void InstructionDebug() => this.instructions.associatedMethod.Log(LogTypes.Info, this.instructions);

        public override string ToString() => this.instructions.associatedMethod.Fullname;

        internal void RemoveLast() => this.instructions.instructions.RemoveAt(this.instructions.instructions.Count - 1);
    }
}