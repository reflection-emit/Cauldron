using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Coders
{
    internal class InstructionMarker
    {
        public BuilderType exceptionType;
        public Instruction instruction;
        public MarkerType markerType;
    }
}