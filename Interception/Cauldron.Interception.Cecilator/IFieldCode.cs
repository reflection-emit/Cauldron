using System.ComponentModel;

namespace Cauldron.Interception.Cecilator
{
    public interface IFieldCode
    {
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

        ICode NewObj(AttributedType attribute);

        ICode NewObj(AttributedProperty attribute);

        ICode NewObj(AttributedMethod attribute);

        ICode NewObj(Method constructor, params object[] parameters);

        ICode NewObj(AttributedField attribute);

        ICode Return();

        ICode Set(object value);

        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();
    }
}