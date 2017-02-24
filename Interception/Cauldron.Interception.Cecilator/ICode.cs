using System;
using System.ComponentModel;

namespace Cauldron.Interception.Cecilator
{
    public interface ICode : IAction
    {
        Crumb Parameters { get; }

        Crumb This { get; }

        IFieldCode Assign(Field field);

        ILocalVariableCode Assign(LocalVariable localVariable);

        IFieldCode AssignToField(string fieldName);

        ILocalVariableCode AssignToLocalVariable(string localVariableName);

        ILocalVariableCode AssignToLocalVariable(int localVariableIndex);

        ICode Call(Method method, params object[] parameters);

        ICode Callvirt(Method method, params object[] parameters);

        ICode Context(Action<ICode> body);

        Method Copy(Modifiers modifiers, string newName);

        LocalVariable CreateVariable(BuilderType type);

        LocalVariable CreateVariable(Method method);

        LocalVariable CreateVariable(string name, Method method);

        LocalVariable CreateVariable(string name, BuilderType type);

        LocalVariable CreateVariable(Type type);

        [EditorBrowsable(EditorBrowsableState.Never)]
        new bool Equals(object obj);

        IIfCode EqualTo(long value);

        IIfCode EqualTo(int value);

        IIfCode EqualTo(bool value);

        [EditorBrowsable(EditorBrowsableState.Never)]
        new int GetHashCode();

        ILocalVariableCode Load(LocalVariable localVariable);

        //IIfCode LesserThan(long value);
        IFieldCode Load(Field field);

        //IIfCode LesserThan(int value);
        ICode Load(Crumb crumb);

        //IIfCode GreaterThan(long value);
        IFieldCode LoadField(string fieldName);

        //IIfCode GreaterThan(int value);
        ILocalVariableCode LoadVariable(string variableName);

        ILocalVariableCode LoadVariable(int variableIndex);

        ICode Pop();

        ICode Return();

        [EditorBrowsable(EditorBrowsableState.Never)]
        new string ToString();

        ITry Try(Action<ITryCode> body);
    }
}