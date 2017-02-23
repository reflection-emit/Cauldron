using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System;

namespace Cauldron.Interception.Cecilator
{
    internal class InstructionContainer
    {
        private readonly List<ExceptionHandler> exceptionHandlers = new List<ExceptionHandler>();
        private readonly List<Instruction> instruction = new List<Instruction>();
        private readonly VariableDefinitionKeyedCollection variables = new VariableDefinitionKeyedCollection();

        public InstructionContainer()
        {
        }

        private InstructionContainer(InstructionContainer a, InstructionContainer b)
        {
            this.instruction.AddRange(a.instruction);
            this.instruction.AddRange(b.instruction);
        }

        private InstructionContainer(InstructionContainer a, IEnumerable<Instruction> b)
        {
            this.instruction.AddRange(a.instruction);
            this.instruction.AddRange(b);
        }

        public int Count { get { return this.instruction.Count; } }

        public List<ExceptionHandler> ExceptionHandlers { get { return this.exceptionHandlers; } }

        public VariableDefinitionKeyedCollection Variables { get { return this.variables; } }

        public Instruction this[int index] { get { return this.instruction[index]; } }

        public static implicit operator Instruction[] (InstructionContainer a) => a.instruction.ToArray();

        public static InstructionContainer operator +(InstructionContainer a, InstructionContainer b) => new InstructionContainer(a, b);

        public static InstructionContainer operator +(InstructionContainer a, IEnumerable<Instruction> b) => new InstructionContainer(a, b);

        public void Append(Instruction instruction) => this.instruction.Add(instruction);

        public void Append(IEnumerable<Instruction> instruction) => this.instruction.AddRange(instruction);

        public void Clear()
        {
            this.instruction.Clear();
            this.exceptionHandlers.Clear();
            this.variables.Clear();
        }

        public Instruction First() => this.instruction.Count == 0 ? null : this.instruction[0];

        public void Insert(int index, Instruction item) => this.instruction.Insert(index, item);

        public void Insert(int index, IEnumerable<Instruction> items)
        {
            foreach (var item in items)
                this.instruction.Insert(index++, item);
        }

        public void InsertAfter(Instruction position, Instruction instructionToInsert)
        {
            var index = this.instruction.IndexOf(position) + 1;

            if (index == this.instruction.Count)
                this.instruction.Add(instructionToInsert);
            else
                this.instruction.Insert(index, instructionToInsert);
        }

        public Instruction Last() => this.instruction.Count == 0 ? null : this.instruction[this.instruction.Count - 1];

        public Instruction Next(Instruction instruction)
        {
            var index = this.instruction.IndexOf(instruction);
            return this.instruction[index + 1];
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var item in this.instruction)
                sb.AppendLine($"IL_{item.Offset.ToString("X4")}: {item.OpCode.ToString()} { (item.Operand is Instruction ? "IL_" + (item.Operand as Instruction).Offset.ToString("X4") : item.Operand?.ToString())} ");

            return sb.ToString();
        }
    }
}