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

        public Method Method => this.method;

        public Position Next => new Position(this.method, this.instruction.Next);
        public Position Previous => new Position(this.method, this.instruction.Previous);

        public bool IsOpCode(OpCode opcode) => this.instruction.OpCode == opcode;

        public override string ToString() => this.instruction.ToString();
    }
}