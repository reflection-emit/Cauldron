using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator
{
    public interface ICecilatorObject
    {
        void Log(LogTypes logTypes, Instruction instruction, MethodDefinition methodDefinition, object arg);

        void Log(LogTypes logTypes, MethodDefinition method, object arg);

        void Log(LogTypes logTypes, SequencePoint sequencePoint, object arg);
    }
}