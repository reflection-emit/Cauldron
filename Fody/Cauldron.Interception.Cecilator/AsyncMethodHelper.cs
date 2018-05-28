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

        static AsyncMethodHelper()
        {
            asyncStateMachineAttribute = Builder.Current.GetType("System.Runtime.CompilerServices.AsyncStateMachineAttribute");
            iAsyncStateMachine = Builder.Current.GetType("System.Runtime.CompilerServices.IAsyncStateMachine");
        }

        internal AsyncMethodHelper(Method method) : base(method) => this.method = method;

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

                var methodOriginType = this.method.OriginType;
                var asyncMachine = asyncStateMachineAttribute.typeDefinition;
                return originType
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

            var lastGetResult = asyncMethod.methodDefinition.Body.Instructions.Last(x =>
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

            return new Position(asyncMethod, lastGetResult.Previous.Previous);
        }

        /// <summary>
        /// The position of the last SetResult method call starting from the first parameter in the AsyncStateMachine MoveNext method.
        /// </summary>
        /// <returns></returns>
        public Position GetAsyncStateMachineLastSetResult()
        {
            var asyncMethod = this.method.AsyncMethod ?? this.method;

            if (asyncMethod == null)
                return null;

            var lastSetResult = asyncMethod.methodDefinition.Body.Instructions.Last(x =>
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

            return new Position(asyncMethod, lastSetResult.Previous.Previous.Previous);
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

        public Field InsertFieldToAsyncStateMachine(string fieldName, BuilderType fieldType, Func<Coder, object> setCoder) =>
            InsertFieldToAsyncStateMachine(fieldName, fieldType.typeReference, setCoder);

        public Field InsertFieldToAsyncStateMachine(string fieldName, TypeReference fieldType, Func<Coder, object> setCoder)
        {
            var result = this.method.AsyncMethod.OriginType.GetField(fieldName, false);
            if (result != null)
                return result;

            var newField = this.method.AsyncMethod.OriginType.CreateField(Modifiers.Public, fieldType, fieldName);
            var resolvedField = newField.Resolve(this.method);
            var position = this.GetAsyncTaskMethodBuilderInitialization();

            if (position == null)
                this.method.NewCoder().Load(variable: x => x.GetVariable(0)).SetValue(resolvedField, setCoder).Insert(InsertionPosition.Beginning);
            else
                this.method.NewCoder().Load(variable: x => x.GetVariable(0)).SetValue(resolvedField, setCoder).Insert(InsertionAction.After, position);

            return newField.Resolve(this.method.AsyncMethod.DeclaringType);
        }

        private Field AddThisReference() => this.InsertFieldToAsyncStateMachine("<>4__this", this.method.OriginType, x => CodeBlocks.This);
    }
}