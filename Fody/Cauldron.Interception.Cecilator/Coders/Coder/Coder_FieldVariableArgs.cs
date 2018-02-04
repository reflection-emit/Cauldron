namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class Coder
    {
        public FieldAssignCoder Load(Field field) => new FieldAssignCoder(this, field);

        public LocalVariableAssignCoder Load(LocalVariable localVariable) => new LocalVariableAssignCoder(this, localVariable);

        public ArgAssignCoder LoadArg(string argname) => new ArgAssignCoder(this, CodeBlocks.GetParameter(argname));

        public ArgAssignCoder LoadArg(int argIndex) => new ArgAssignCoder(this, CodeBlocks.GetParameter(argIndex));

        public FieldAssignCoder LoadField(string fieldname) => this.Load(this.instructions.associatedMethod.DeclaringType.GetField(fieldname));

        public LocalVariableAssignCoder LoadVariable(BuilderType variableType, string variableName) =>
           new LocalVariableAssignCoder(this, this.instructions.associatedMethod.CreateVariable(variableType.typeReference, variableName));

        public LocalVariableAssignCoder LoadVariable(string variableName) =>
           new LocalVariableAssignCoder(this, this.instructions.associatedMethod.CreateVariable(BuilderType.Object.typeReference, variableName));
    }
}