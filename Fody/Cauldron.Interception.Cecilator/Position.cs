using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator
{
    public sealed class Position
    {
        internal readonly Instruction instruction;
        internal readonly Method method;

        internal Position(Method method, Instruction instruction)
        {
            this.method = method;
            this.instruction = instruction;
        }

        public int Index => this.method.methodDefinition.Body.Instructions.IndexOf(this.instruction);
        public Method Method => this.method;
        public Position Next => new Position(this.method, this.instruction.Next);
        public Position Previous => new Position(this.method, this.instruction.Previous);

        public bool IsOpCode(OpCode opcode) => this.instruction.OpCode == opcode;

        public override string ToString() =>
            $"IL_{instruction.Offset.ToString("X4")}: {instruction.OpCode.ToString()} { (instruction.Operand is Instruction ? "IL_" + (instruction.Operand as Instruction).Offset.ToString("X4") : instruction.Operand?.ToString())} ";
    }
}