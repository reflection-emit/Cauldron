using Cauldron.Interception.Cecilator.Coders;
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

                return (object)CodeBlocks.This;
            }
        }

        public Method Method
        {
            get
            {
                var originType = this.OriginType;
                if (originType == this.method.OriginType)
                    return this.method;
                var asyncMachine = Builder.Current.GetType("System.Runtime.CompilerServices.AsyncStateMachineAttribute").typeDefinition;
                return originType
                      .GetMethods()
                      .Where(x => x.methodDefinition.HasCustomAttributes && x.methodDefinition.CustomAttributes.HasAttribute(asyncMachine))
                      .Select(x => new
                      {
                          Attribute = x.CustomAttributes.FirstOrDefault(y => y.attribute.AttributeType.AreEqual(asyncMachine)),
                          Method = x
                      })
                      .FirstOrDefault(x => (x.Attribute.ConstructorArguments[0].Value as TypeReference).AreEqual(this.method.OriginType))
                      ?.Method;
            }
        }

        public BuilderType OriginType
        {
            get
            {
                if (this.method.AsyncMethod == null && this.method.OriginType.IsAsyncStateMachine)
                {
                    var result = this.method.OriginType.Fields.FirstOrDefault(x => x.Name == "<>4__this");
                    if (result == null)
                        throw new Exception("This is not possible.");

                    return result.FieldType;
                }
                else if (this.method.AsyncMethod != null)
                {
                    var result = this.method.AsyncMethod.OriginType.Fields.FirstOrDefault(x => x.Name == "<>4__this");
                    if (result == null)
                        return this.AddThisReference()?.FieldType;

                    return result.FieldType;
                }

                return this.method.OriginType;
            }
        }

        public Tuple<Positions, ExceptionHandler> GetAsyncStateMachineExceptionBlock()
        {
            var asyncMethod = this.method.AsyncMethod ?? this.method;

            if (asyncMethod == null)
                return new Tuple<Positions, ExceptionHandler>(new Positions { Beginning = null, End = null }, null);

            var lastException = asyncMethod.methodDefinition.Body.ExceptionHandlers.Last(x => x.HandlerType == ExceptionHandlerType.Catch);

            return
                new Tuple<Positions, ExceptionHandler>(
                new Positions
                {
                    Beginning = new Position(asyncMethod, lastException.HandlerStart),
                    End = new Position(asyncMethod, lastException.HandlerEnd)
                },
                lastException);
        }

        public LocalVariable GetAsyncStateMachineExceptionVariable()
        {
            var asyncMethod = this.method.AsyncMethod ?? this.method;

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

            if (variable == null)
                throw new NullReferenceException("Unable to find the state machines exception variable.");

            return new LocalVariable(asyncMethod.type, variable);
        }

        /// <summary>
        /// The position of the last GetResult method call starting from the first parameter in the AsyncStateMachine MoveNext method.
        /// </summary>
        /// <returns></returns>
        public Position GetAsyncStateMachineLastGetResult()
        {
            var asyncMethod = this.method.AsyncMethod ?? this.method;

            if (asyncMethod == null)
                return null;

            var lastException = asyncMethod.methodDefinition.Body.Instructions.Last(x =>
            {
                if (x.OpCode == OpCodes.Call)
                {
                    var method = x.Operand as MethodDefinition ?? x.Operand as MethodReference;
                    if (method != null && method.Name == "GetResult")
                        return true;
                }

                return false;
            });

            if (lastException == null)
                return null;

            return new Position(asyncMethod, lastException.Previous.Previous);
        }

        public Positions GetAsyncStateMachineTryBlock()
        {
            var asyncMethod = this.method.AsyncMethod ?? this.method;

            if (asyncMethod == null)
                return new Positions { Beginning = null, End = null };

            var lastException = asyncMethod.methodDefinition.Body.ExceptionHandlers.Last(x => x.HandlerType == ExceptionHandlerType.Catch);

            return new Positions
            {
                Beginning = new Position(asyncMethod, lastException.TryStart),
                End = new Position(asyncMethod, lastException.TryEnd)
            };
        }

        public Position GetAsyncTaskMethodBuilderInitialization()
        {
            var asyncMethod = this.Method;
            var result = asyncMethod.methodDefinition.Body.Instructions
                .FirstOrDefault(x => x.OpCode == OpCodes.Stfld && (x.Operand as FieldDefinition ?? x.Operand as FieldReference).Name == "<>1__state");

            // If the result is null then probably the helper class is a struct
            if (result == null)
                return null;

            return new Position(this.method, result);
        }

        public Position GetAsyncTaskMethodBuilderStart()
        {
            var asyncMethod = this.method.AsyncMethod ?? this.method;
            var result = asyncMethod.methodDefinition.Body.Instructions
                .FirstOrDefault(x => x.OpCode == OpCodes.Call && (x.Operand as MethodDefinition ?? x.Operand as MethodReference).Name == "Start");

            // If the result is null then probably the helper class is a struct
            if (result == null)
                return null;

            return new Position(this.method, result);
        }

        private Field AddThisReference()
        {
            var thisField = this.method.AsyncMethod.OriginType.CreateField(Modifiers.Public, this.method.OriginType, "<>4__this").Resolve(this.method);
            var position = this.GetAsyncTaskMethodBuilderInitialization();

            if (position == null)
                this.method.NewCoder().Load(variable: x => x.GetVariable(0)).SetValue(thisField, CodeBlocks.This).Insert(InsertionPosition.Beginning);
            else
                this.method.NewCoder().Load(variable: x => x.GetVariable(0)).SetValue(thisField, CodeBlocks.This).Insert(InsertionAction.After, position);

            return thisField;
        }
    }
}