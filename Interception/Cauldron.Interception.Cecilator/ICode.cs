using System;
using System.ComponentModel;

namespace Cauldron.Interception.Cecilator
{
    public interface ICode : IAction
    {
        IFieldCode Assign(Field field);

        ILocalVariableCode Assign(LocalVariable localVariable);

        IFieldCode AssignToField(string fieldName);

        ILocalVariableCode AssignToLocalVariable(string localVariableName);

        ILocalVariableCode AssignToLocalVariable(int localVariableIndex);

        ICode Call(Method method, params object[] parameters);

        ICode Callvirt(Method method, params object[] parameters);

        ICode Context(Action<ICode> body);

        [EditorBrowsable(EditorBrowsableState.Never)]
        new bool Equals(object obj);

        [EditorBrowsable(EditorBrowsableState.Never)]
        new int GetHashCode();

        ILocalVariableCode Load(LocalVariable localVariable);

        IFieldCode Load(Field field);

        IFieldCode LoadField(string fieldName);

        ILocalVariableCode LoadLocalVariable(int variableIndex);

        ILocalVariableCode LoadLocalVariable(string variableName);

        ICode Return();

        [EditorBrowsable(EditorBrowsableState.Never)]
        new string ToString();

        ITry Try(Action<ITryCode> body);
    }
}