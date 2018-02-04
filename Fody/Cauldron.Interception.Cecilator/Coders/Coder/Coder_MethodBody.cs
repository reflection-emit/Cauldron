using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class Coder
    {
        public LocalVariable CreateVariable(string name, Method method)
        {
            if (method.IsCtor)
                return this.CreateVariable(name, method.OriginType);

            return this.CreateVariable(name, method.ReturnType);
        }

        public LocalVariable CreateVariable(string name, BuilderType type)
        {
            var existingVariable = this.instructions.associatedMethod.GetVariable(name);

            if (existingVariable != null)
                return new LocalVariable(this.instructions.associatedMethod.type, existingVariable, name);

            var newVariable = new VariableDefinition(Builder.Current.Import(type.typeReference));
            this.instructions.associatedMethod.AddLocalVariable(name, newVariable);

            return new LocalVariable(this.instructions.associatedMethod.type, newVariable, name);
        }

        public LocalVariable CreateVariable(Type type)
        {
            var newVariable = new VariableDefinition(Builder.Current.Import(type.GetTypeDefinition()));
            var name = Coder.VariablePrefix + Coder.GenerateName();

            this.instructions.associatedMethod.AddLocalVariable(name, newVariable);
            return new LocalVariable(this.instructions.associatedMethod.type, newVariable, name);
        }

        public LocalVariable CreateVariable(Method method)
        {
            if (method.IsCtor)
                return this.CreateVariable(method.OriginType);

            return this.CreateVariable(method.ReturnType);
        }

        public LocalVariable CreateVariable(TypeReference type)
        {
            var newVariable = new VariableDefinition(Builder.Current.Import(type));
            var name = Coder.VariablePrefix + Coder.GenerateName();

            this.instructions.associatedMethod.AddLocalVariable(name, newVariable);
            return new LocalVariable(this.instructions.associatedMethod.type, newVariable, name);
        }

        public LocalVariable CreateVariable(BuilderType type)
        {
            var newVariable = new VariableDefinition(Builder.Current.Import(type.typeReference));
            var name = Coder.VariablePrefix + Coder.GenerateName();

            this.instructions.associatedMethod.AddLocalVariable(name, newVariable);
            return new LocalVariable(this.instructions.associatedMethod.type, newVariable, name);
        }

        /// <summary>
        /// Gets or creates a return variable.
        /// This will try to detect the existing return variable and create a new return variable if not found.
        /// </summary>
        /// <param name="coder">The coder to use.</param>
        /// <returns>A return variable.</returns>
        public VariableDefinition GetOrCreateReturnVariable()
        {
            var variable = this.instructions.associatedMethod.GetVariable(Coder.ReturnVariableName);

            if (variable != null)
                return variable;

            if (this.instructions.associatedMethod.methodDefinition.Body.Instructions.Count > 1)
            {
                var lastOpCode = this.instructions.associatedMethod.methodDefinition.Body.Instructions.Last().Previous;

                if (lastOpCode.IsLoadLocal())
                {
                    if (lastOpCode.Operand is int index && this.instructions.associatedMethod.methodDefinition.Body.Variables.Count > index)
                        variable = this.instructions.associatedMethod.methodDefinition.Body.Variables[index];

                    if (variable == null && lastOpCode.Operand is VariableDefinition variableReference)
                        variable = variableReference;

                    if (variable == null)
                        if (lastOpCode.OpCode == OpCodes.Ldloc_0) variable = this.instructions.associatedMethod.methodDefinition.Body.Variables[0];
                        else if (lastOpCode.OpCode == OpCodes.Ldloc_1) variable = this.instructions.associatedMethod.methodDefinition.Body.Variables[1];
                        else if (lastOpCode.OpCode == OpCodes.Ldloc_2) variable = this.instructions.associatedMethod.methodDefinition.Body.Variables[2];
                        else if (lastOpCode.OpCode == OpCodes.Ldloc_3) variable = this.instructions.associatedMethod.methodDefinition.Body.Variables[3];

                    if (variable != null)
                    {
                        this.instructions.associatedMethod.AddLocalVariable(Coder.ReturnVariableName, variable);
                        return variable;
                    }
                }
            }

            return this.instructions.associatedMethod.AddLocalVariable(Coder.ReturnVariableName, new VariableDefinition(this.instructions.associatedMethod.ReturnType.typeReference));
        }

        public bool HasReturnVariable() => this.instructions.associatedMethod.GetVariable(Coder.ReturnVariableName) != null;

        public Coder Load(object value)
        {
            this.instructions.Append(InstructionBlock.CreateCode(this.instructions, null, value));

            if (value != null && value is ArrayCodeBlock arrayCodeSet)
            {
                this.instructions.Append(InstructionBlock.CreateCode(this.instructions, null, arrayCodeSet.index));
                this.instructions.Emit(OpCodes.Ldelem_Ref);
            }

            return this;
        }

        public Coder Return()
        {
            this.instructions.Emit(OpCodes.Ret);
            return this;
        }

        public Coder ReturnDefault()
        {
            if (!this.instructions.associatedMethod.IsVoid)
            {
                var variable = this.GetOrCreateReturnVariable();
                var defaultValue = this.instructions.associatedMethod.ReturnType.DefaultValue;

                this.instructions.Append(InstructionBlock.CreateCode(this.instructions,
                    this.instructions.associatedMethod.ReturnType.GenericArguments().Any() ?
                       this.instructions.associatedMethod.ReturnType.GetGenericArgument(0) :
                       this.instructions.associatedMethod.ReturnType, defaultValue));
            }

            this.Return();

            return this;
        }

        public Coder ThrowNew(Type exception)
        {
            this.instructions.Emit(OpCodes.Newobj, Builder.Current.Import(Builder.Current.Import(exception).GetMethodReference(".ctor", 0)));
            this.instructions.Emit(OpCodes.Throw);
            return this;
        }

        public Coder ThrowNew(Type exception, string message)
        {
            this.instructions.Emit(OpCodes.Ldstr, message);
            this.instructions.Emit(OpCodes.Newobj, Builder.Current.Import(Builder.Current.Import(exception).GetMethodReference(".ctor", new Type[] { typeof(string) })));
            this.instructions.Emit(OpCodes.Throw);
            return this;
        }

        public Coder ThrowNew(Method ctor, params object[] parameters)
        {
            // TODO

            this.instructions.Emit(OpCodes.Throw);
            return this;
        }
    }
}