using System.Collections.Generic;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public static class LocalVariableCoderExtensions
    {
        internal static LocalVariableAssignCoder CreateLocalVariableInstructionSet(this Coder coder, LocalVariable localVariable, AssignInstructionType instructionType) =>
            new LocalVariableAssignCoder(coder, localVariable, instructionType);

        internal static LocalVariableAssignCoder CreateLocalVariableInstructionSet(this LocalVariableAssignCoder coder, LocalVariable localVariable, AssignInstructionType instructionType)
        {
            var newList = new List<LocalVariable>();
            newList.AddRange(coder.target);
            newList.Add(localVariable);

            return new LocalVariableAssignCoder(coder, newList, instructionType);
        }
    }
}