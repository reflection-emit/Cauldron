using System.Collections.Generic;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public static class FieldCoderExtensions
    {
        internal static FieldAssignCoder CreateFieldInstructionSet(this Coder coder, Field field, AssignInstructionType instructionType) =>
            new FieldAssignCoder(coder, field, instructionType);

        internal static FieldAssignCoder CreateFieldInstructionSet(this FieldAssignCoder coder, Field field, AssignInstructionType instructionType)
        {
            var newList = new List<Field>();
            newList.AddRange(coder.target);
            newList.Add(field);

            return new FieldAssignCoder(coder, newList, instructionType);
        }
    }
}