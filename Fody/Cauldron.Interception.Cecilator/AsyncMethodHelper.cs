using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class AsyncMethodHelper : CecilatorBase
    {
        private readonly Method method;

        internal AsyncMethodHelper(Method method) : base(method) => this.method = method;

        public object Instance
        {
            get
            {
                if (this.method.AsyncMethod == null && this.method.OriginType.IsAsyncStateMachine)
                {
                    var result = this.method.OriginType.Fields.FirstOrDefault(x => x.Name == "<>4__this");
                    if (result == null)
                        throw new Exception("This is not possible.");

                    return result;
                }
                else if (this.method.AsyncMethod != null)
                {
                    var result = this.method.AsyncMethod.OriginType.Fields.FirstOrDefault(x => x.Name == "<>4__this");
                    if (result == null)
                        return this.AddThisReference();

                    return result;
                }

                return (object)Crumb.This;
            }
        }

        public (Position Start, Position End) GetAsyncStateMachineExceptionBlock()
        {
            var asyncMethod = this.method.AsyncMethod;

            if (asyncMethod == null)
                return (null, null);

            var lastException = asyncMethod.methodDefinition.Body.ExceptionHandlers.Last(x => x.HandlerType == ExceptionHandlerType.Catch);

            return (new Position(asyncMethod, lastException.HandlerStart), new Position(asyncMethod, lastException.HandlerEnd));
        }

        public LocalVariable GetAsyncStateMachineExceptionVariable()
        {
            var asyncMethod = this.method.AsyncMethod;

            if (asyncMethod == null)
                return null;

            var lastException = asyncMethod.methodDefinition.Body.ExceptionHandlers.Last(x => x.HandlerType == ExceptionHandlerType.Catch);
            var lastOpCode = lastException.HandlerStart;
            VariableDefinition variable = null;

            if (lastOpCode.Operand is int index && asyncMethod.methodDefinition.Body.Variables.Count > index)
                variable = asyncMethod.methodDefinition.Body.Variables[index];

            if (variable == null && lastOpCode.Operand is VariableDefinition variableReference)
                variable = variableReference;

            if (variable == null)
                if (lastOpCode.OpCode == OpCodes.Stloc_0) variable = asyncMethod.methodDefinition.Body.Variables[0];
                else if (lastOpCode.OpCode == OpCodes.Stloc_1) variable = asyncMethod.methodDefinition.Body.Variables[1];
                else if (lastOpCode.OpCode == OpCodes.Stloc_2) variable = asyncMethod.methodDefinition.Body.Variables[2];
                else if (lastOpCode.OpCode == OpCodes.Stloc_3) variable = asyncMethod.methodDefinition.Body.Variables[3];

            return new LocalVariable(asyncMethod.type, variable);
        }

        public Position GetAsyncTaskMethodBuilderInitialization()
        {
            var result = this.method.methodDefinition.Body.Instructions.FirstOrDefault(x => x.OpCode == OpCodes.Newobj && (x.Operand as MethodDefinition ?? x.Operand as MethodReference).Name == ".ctor");

            // If the result is null then probably the helper class is a struct
            if (result == null)
                return null;

            return new Position(this.method, result);
        }

        private Field AddThisReference()
        {
            var thisField = this.method.AsyncMethod.OriginType.CreateField(Modifiers.Public, this.method.OriginType, "<>4__this");
            var position = this.GetAsyncTaskMethodBuilderInitialization();

            if (position == null)
                this.method.NewCode().LoadVariable(0).Assign(thisField).Set(Crumb.This).Insert(InsertionPosition.Beginning);
            else
                this.method.NewCode().LoadVariable(0).Assign(thisField).Set(Crumb.This).Insert(InsertionAction.After, position);

            return thisField;
        }
    }
}