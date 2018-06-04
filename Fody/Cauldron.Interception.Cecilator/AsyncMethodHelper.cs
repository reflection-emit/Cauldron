using Cauldron.Interception.Cecilator.Coders;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class AsyncMethodHelper : CecilatorBase
    {
        private static BuilderType asyncStateMachineAttribute;
        private static BuilderType iAsyncStateMachine;
        private readonly Method method;

        private Method asyncMethod;
        private Method moveNextMethod;

        static AsyncMethodHelper()
        {
            asyncStateMachineAttribute = Builder.Current.GetType("System.Runtime.CompilerServices.AsyncStateMachineAttribute");
            iAsyncStateMachine = Builder.Current.GetType("System.Runtime.CompilerServices.IAsyncStateMachine");
        }

        internal AsyncMethodHelper(Method method) : base(method) => this.method = method;

        /// <summary>
        /// Gets the async state machine
        /// </summary>
        public BuilderType AsyncStateMachineType => this.MoveNextMethod.OriginType;

        public bool HasAsyncStateMachineAttribute
        {
            get
            {
                if (this.method.CustomAttributes.Any(x => x.Type == asyncStateMachineAttribute))
                    return true;

                if (this.method.OriginType.Implements(iAsyncStateMachine))
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Return "this" if the method is the async method; otherwise the field that references to the async method's class.
        /// </summary>
        public object Instance
        {
            get
            {
                if (this.method.OriginType.IsAsyncStateMachine)
                {
                    var result = this.AsyncStateMachineType.GetField("<>4__this", false);
                    if (result == null)
                        result = this.AddThisReference();

                    return result;
                }

                return (object)CodeBlocks.This;
            }
        }

        /// <summary>
        /// Gets the async method
        /// </summary>
        public Method Method
        {
            get
            {
                if (this.asyncMethod == null)
                {
                    if (this.method.CustomAttributes.Any(x => x.Type == asyncStateMachineAttribute))
                    {
                        this.asyncMethod = this.method;
                        return this.asyncMethod;
                    }
                    else if (this.method.Name == "MoveNext" && this.method.OriginType.IsAsyncStateMachine)
                    {
                        var methodOriginType = this.method.OriginType;
                        var originType = this.method.OriginType.GetNestedTypeParent();
                        var asyncMachine = asyncStateMachineAttribute.typeDefinition;
                        this.asyncMethod = originType
                              .GetMethods()
                              .FirstOrDefault(x =>
                              {
                                  if (!x.methodDefinition.HasCustomAttributes)
                                      return false;

                                  for (int i = 0; i < x.methodDefinition.CustomAttributes.Count; i++)
                                      if (x.methodDefinition.CustomAttributes[i].AttributeType.AreEqual(asyncMachine) &&
                                        (x.methodDefinition.CustomAttributes[i].ConstructorArguments[0].Value as TypeReference).AreEqual(methodOriginType))
                                          return true;

                                  return false;
                              });

                        return this.asyncMethod;
                    }
                    else // This happens if this property is called and the method is not async at all
                        this.asyncMethod = this.method;
                }

                return this.asyncMethod;
            }
        }

        /// <summary>
        /// Gets the MoveNext method in the async state machine
        /// </summary>
        public Method MoveNextMethod
        {
            get
            {
                if (this.moveNextMethod == null)
                {
                    if (this.method.Name == "MoveNext" && this.method.OriginType.IsAsyncStateMachine)
                        this.moveNextMethod = this.method;
                    else if (this.method.CustomAttributes.Any(x => x.Type == asyncStateMachineAttribute))
                        this.moveNextMethod = this.method.AsyncMethod;
                }

                return this.moveNextMethod;
            }
        }

        /// <summary>
        /// Gets the type that inherited the async method.
        /// </summary>
        public BuilderType OriginType => this.Method.OriginType;

        public Tuple<Positions, ExceptionHandler> GetAsyncStateMachineExceptionBlock()
        {
            var lastException = this.MoveNextMethod.methodDefinition.Body.ExceptionHandlers.Last(x => x.HandlerType == ExceptionHandlerType.Catch);

            return
                new Tuple<Positions, ExceptionHandler>(
                new Positions
                {
                    Beginning = new Position(this.MoveNextMethod, lastException.HandlerStart),
                    End = new Position(this.MoveNextMethod, lastException.HandlerEnd)
                },
                lastException);
        }

        public LocalVariable GetAsyncStateMachineExceptionVariable()
        {
            var lastException = this.MoveNextMethod.methodDefinition.Body.ExceptionHandlers.Last(x => x.HandlerType == ExceptionHandlerType.Catch);
            var lastOpCode = lastException.HandlerStart;
            VariableDefinition variable = null;

            if (lastOpCode.Operand is int index && this.MoveNextMethod.methodDefinition.Body.Variables.Count > index)
                variable = this.MoveNextMethod.methodDefinition.Body.Variables[index];

            if (variable == null && lastOpCode.Operand is VariableDefinition variableReference)
                variable = variableReference;

            if (variable == null)
                if (lastOpCode.OpCode == OpCodes.Stloc_0) variable = this.MoveNextMethod.methodDefinition.Body.Variables[0];
                else if (lastOpCode.OpCode == OpCodes.Stloc_1) variable = this.MoveNextMethod.methodDefinition.Body.Variables[1];
                else if (lastOpCode.OpCode == OpCodes.Stloc_2) variable = this.MoveNextMethod.methodDefinition.Body.Variables[2];
                else if (lastOpCode.OpCode == OpCodes.Stloc_3) variable = this.MoveNextMethod.methodDefinition.Body.Variables[3];

            if (variable == null)
                throw new NullReferenceException("Unable to find the state machines exception variable.");

            return new LocalVariable(this.MoveNextMethod.type, variable);
        }

        /// <summary>
        /// The position of the last GetResult method call starting from the first parameter in the AsyncStateMachine MoveNext method.
        /// </summary>
        /// <returns></returns>
        public Position GetAsyncStateMachineLastGetResult()
        {
            var lastGetResult = this.MoveNextMethod.methodDefinition.Body.Instructions.Last(x =>
            {
                if (x.OpCode == OpCodes.Call)
                {
                    var method = x.Operand as MethodDefinition ?? x.Operand as MethodReference;
                    if (method != null && method.Name == "GetResult")
                        return true;
                }

                return false;
            });

            if (lastGetResult == null)
                return null;

            return new Position(this.MoveNextMethod, lastGetResult.Previous.Previous);
        }

        /// <summary>
        /// The position of the last SetResult method call starting from the first parameter in the AsyncStateMachine MoveNext method.
        /// </summary>
        /// <returns></returns>
        public Position GetAsyncStateMachineLastSetResult()
        {
            var lastSetResult = this.MoveNextMethod.methodDefinition.Body.Instructions.Last(x =>
            {
                if (x.OpCode == OpCodes.Call)
                {
                    var method = x.Operand as MethodDefinition ?? x.Operand as MethodReference;
                    if (method != null && method.Name == "SetResult")
                        return true;
                }

                return false;
            });

            if (lastSetResult == null)
                return null;

            return new Position(this.MoveNextMethod, lastSetResult.Previous.Previous.Previous);
        }

        public Positions GetAsyncStateMachineTryBlock()
        {
            var lastException = this.MoveNextMethod.methodDefinition.Body.ExceptionHandlers.Last(x => x.HandlerType == ExceptionHandlerType.Catch);

            return new Positions
            {
                Beginning = new Position(this.MoveNextMethod, lastException.TryStart),
                End = new Position(this.MoveNextMethod, lastException.TryEnd)
            };
        }

        public Position GetAsyncTaskMethodBuilderInitialization()
        {
            var result = this.Method.methodDefinition.Body.Instructions
                .FirstOrDefault(x => x.OpCode == OpCodes.Stfld && (x.Operand as FieldDefinition ?? x.Operand as FieldReference).Name == "<>1__state");

            // If the result is null then probably the helper class is a struct
            if (result == null)
                return null;

            return new Position(this.method, result);
        }

        public Position GetAsyncTaskMethodBuilderStart()
        {
            var result = this.Method.methodDefinition.Body.Instructions
                .FirstOrDefault(x => x.OpCode == OpCodes.Call && (x.Operand as MethodDefinition ?? x.Operand as MethodReference).Name == "Start");

            // If the result is null then probably the helper class is a struct
            if (result == null)
                return null;

            return new Position(this.method, result);
        }

        public Field InsertFieldToAsyncStateMachine(string fieldName, BuilderType fieldType, Func<Coder, object> setCoder) =>
            InsertFieldToAsyncStateMachine(fieldName, fieldType.typeReference, setCoder);

        public Field InsertFieldToAsyncStateMachine(string fieldName, TypeReference fieldType, Func<Coder, object> setCoder)
        {
            var result = this.AsyncStateMachineType.GetField(fieldName, false);
            if (result != null)
                return result;

            var newField = this.AsyncStateMachineType.CreateField(Modifiers.Public, fieldType, fieldName);
            var resolvedField = newField.Resolve(this.Method);
            var position = this.GetAsyncTaskMethodBuilderInitialization();

            if (position == null)
                this.Method.NewCoder().Load(variable: x => x.GetVariable(0)).SetValue(resolvedField, setCoder).Insert(InsertionPosition.Beginning);
            else
                this.Method.NewCoder().Load(variable: x => x.GetVariable(0)).SetValue(resolvedField, setCoder).Insert(InsertionAction.After, position);

            return newField.Resolve(this.AsyncStateMachineType);
        }

        private Field AddThisReference() => this.InsertFieldToAsyncStateMachine("<>4__this", this.OriginType, x => CodeBlocks.This);
    }
}