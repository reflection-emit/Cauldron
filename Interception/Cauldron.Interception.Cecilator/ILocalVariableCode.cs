using System.ComponentModel;

namespace Cauldron.Interception.Cecilator
{
    public interface ILocalVariableCode
    {
        ICode As(BuilderType type);

        IFieldCode Assign(Field field);

        ILocalVariableCode Assign(LocalVariable localVariable);

        IFieldCode AssignToField(string fieldName);

        ILocalVariableCode AssignToLocalVariable(string localVariableName);

        ILocalVariableCode AssignToLocalVariable(int localVariableIndex);

        ICode Call(Method method, params object[] parameters);

        ICode Callvirt(Method method, params object[] parameters);

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object obj);

        IIfCode EqualTo(long value);

        IIfCode EqualTo(int value);

        IIfCode EqualTo(bool value);

        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        IIfCode IsNotNull();

        IIfCode IsNull();

        ILocalVariableCode Load(LocalVariable localVariable);

        IFieldCode Load(Field field);

        IFieldCode LoadField(string fieldName);

        ILocalVariableCode LoadVariable(int variableIndex);

        ILocalVariableCode LoadVariable(string variableName);

        ICode NewObj(AttributedProperty attribute);

        ICode NewObj(AttributedMethod attribute);

        ICode NewObj(AttributedField attribute);

        ICode NewObj(Method constructor, params object[] parameters);

        ICode OriginalBody();

        ICode Return();

        ICode Set(object value);

        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();
    }
}