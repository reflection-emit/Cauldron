using System;
using System.ComponentModel;

namespace Cauldron.Interception.Cecilator
{
    public interface ICode : IAction
    {
        Crumb This { get; }

        ICode As(BuilderType type);

        IFieldCode Assign(Field field);

        ILocalVariableCode Assign(LocalVariable localVariable);

        IFieldCode AssignToField(string fieldName);

        ILocalVariableCode AssignToLocalVariable(string localVariableName);

        ILocalVariableCode AssignToLocalVariable(int localVariableIndex);

        ICode Call(Method method, params object[] parameters);

        ICode Call(Crumb instance, Method method, params object[] parameters);

        ICode Call(Field instance, Method method, params object[] parameters);

        ICode Call(LocalVariable instance, Method method, params object[] parameters);

        ICode Callvirt(Method method, params object[] parameters);

        ICode Callvirt(Crumb instance, Method method, params object[] parameters);

        ICode Callvirt(Field instance, Method method, params object[] parameters);

        ICode Callvirt(LocalVariable instance, Method method, params object[] parameters);

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

        ICode For(LocalVariable array, Action<ICode, LocalVariable> action);

        [EditorBrowsable(EditorBrowsableState.Never)]
        new int GetHashCode();

        Crumb GetParameter(int index);

        Crumb GetParametersArray();

        LocalVariable GetReturnVariable();

        IIfCode Is(BuilderType type);

        IIfCode Is(Type type);

        IIfCode IsFalse();

        IIfCode IsNotNull();

        IIfCode IsNull();

        IIfCode IsTrue();

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

        ICode NewCode();

        ICode NewObj(AttributedMethod attribute);

        ICode NewObj(Method constructor, params object[] parameters);

        ICode NewObj(AttributedField attribute);

        ICode OriginalBody();

        ICode Pop();

        ICode Return();

        ICode StoreLocal(LocalVariable localVariable);

        ICode ThrowNew(Type exceptionType, string message);

        [EditorBrowsable(EditorBrowsableState.Never)]
        new string ToString();

        ITry Try(Action<ITryCode> body);
    }
}