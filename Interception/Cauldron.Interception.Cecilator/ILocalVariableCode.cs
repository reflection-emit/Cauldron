using System.ComponentModel;

namespace Cauldron.Interception.Cecilator
{
    public interface ILocalVariableCode
    {
        ICode Call(Method method, params object[] parameters);

        ICode Callvirt(Method method, params object[] parameters);

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object obj);

        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        ILocalVariableCode Load(LocalVariable localVariable);

        IFieldCode Load(Field field);

        IFieldCode LoadField(string fieldName);

        ILocalVariableCode LoadVariable(int variableIndex);

        ILocalVariableCode LoadVariable(string variableName);

        ICode NewObj(AttributedMethod attribute);

        ICode NewObj(AttributedField attribute);

        ICode NewObj(Method constructor, params object[] parameters);

        ICode Set(object value);

        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();
    }
}