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
    }
}