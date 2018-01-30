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
            var existingVariable = this.method.GetLocalVariable(name);

            if (existingVariable != null)
                return new LocalVariable(this.method.type, existingVariable, name);

            var newVariable = new VariableDefinition(Builder.Current.Import(type.typeReference));
            this.method.AddLocalVariable(name, newVariable);

            return new LocalVariable(this.method.type, newVariable, name);
        }

        public LocalVariable CreateVariable(Type type)
        {
            var newVariable = new VariableDefinition(Builder.Current.Import(type.GetTypeDefinition()));
            var name = Coder.VariablePrefix + Coder.GenerateName();

            this.method.AddLocalVariable(name, newVariable);
            return new LocalVariable(this.method.type, newVariable, name);
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

            this.method.AddLocalVariable(name, newVariable);
            return new LocalVariable(this.method.type, newVariable, name);
        }

        public LocalVariable CreateVariable(BuilderType type)
        {
            var newVariable = new VariableDefinition(Builder.Current.Import(type.typeReference));
            var name = Coder.VariablePrefix + Coder.GenerateName();

            this.method.AddLocalVariable(name, newVariable);
            return new LocalVariable(this.method.type, newVariable, name);
        }

        /// <summary>
        /// Gets or creates a return variable.
        /// This will try to detect the existing return variable and create a new return variable if not found.
        /// </summary>
        /// <param name="coder">The coder to use.</param>
        /// <returns>A return variable.</returns>
        public VariableDefinition GetOrCreateReturnVariable()
        {
            var variable = this.method.GetLocalVariable(Coder.ReturnVariableName);

            if (variable != null)
                return variable;

            if (this.method.methodDefinition.Body.Instructions.Count > 1)
            {
                var lastOpCode = this.method.methodDefinition.Body.Instructions.Last().Previous;

                if (lastOpCode.IsLoadLocal())
                {
                    if (lastOpCode.Operand is int index && this.method.methodDefinition.Body.Variables.Count > index)
                        variable = this.method.methodDefinition.Body.Variables[index];

                    if (variable == null && lastOpCode.Operand is VariableDefinition variableReference)
                        variable = variableReference;

                    if (variable == null)
                        if (lastOpCode.OpCode == OpCodes.Ldloc_0) variable = this.method.methodDefinition.Body.Variables[0];
                        else if (lastOpCode.OpCode == OpCodes.Ldloc_1) variable = this.method.methodDefinition.Body.Variables[1];
                        else if (lastOpCode.OpCode == OpCodes.Ldloc_2) variable = this.method.methodDefinition.Body.Variables[2];
                        else if (lastOpCode.OpCode == OpCodes.Ldloc_3) variable = this.method.methodDefinition.Body.Variables[3];

                    if (variable != null)
                    {
                        this.method.AddLocalVariable(Coder.ReturnVariableName, variable);
                        return variable;
                    }
                }
            }

            return this.method.AddLocalVariable(Coder.ReturnVariableName, new VariableDefinition(this.method.ReturnType.typeReference));
        }

        public bool HasReturnVariable() => this.method.GetLocalVariable(Coder.ReturnVariableName) != null;

        public Coder Load(object value)
        {
            var inst = this.AddParameter(this.processor, null, value);
            this.instructions.Append(inst.Instructions);

            if (value != null && value is ArrayCodeBlock arrayCodeSet)
            {
                this.instructions.Append(this.AddParameter(this.processor, null, arrayCodeSet.index).Instructions);
                this.instructions.Append(this.processor.Create(OpCodes.Ldelem_Ref));
            }

            return this;
        }

        public Coder Return()
        {
            this.instructions.Append(this.processor.Create(OpCodes.Ret));
            return this;
        }

        public Coder ReturnDefault()
        {
            if (!this.method.IsVoid)
            {
                var variable = this.GetOrCreateReturnVariable();
                var defaultValue = this.method.ReturnType.DefaultValue;
                var inst = this.AddParameter(this.processor,
                    this.method.ReturnType.GenericArguments().Any() ?
                       this.method.ReturnType.GetGenericArgument(0).typeReference :
                       this.method.ReturnType.typeReference, defaultValue);

                this.instructions.Append(inst.Instructions);
            }

            this.Return();

            return this;
        }

        public Coder ThrowNew(Type exception)
        {
            this.instructions.Append(this.processor.Create(OpCodes.Newobj, Builder.Current.Import(Builder.Current.Import(exception).GetMethodReference(".ctor", 0))));
            this.instructions.Append(this.processor.Create(OpCodes.Throw));
            return this;
        }

        public Coder ThrowNew(Type exception, string message)
        {
            this.instructions.Append(this.processor.Create(OpCodes.Ldstr, message));
            this.instructions.Append(this.processor.Create(OpCodes.Newobj, Builder.Current.Import(Builder.Current.Import(exception).GetMethodReference(".ctor", new Type[] { typeof(string) }))));
            this.instructions.Append(this.processor.Create(OpCodes.Throw));
            return this;
        }

        public Coder ThrowNew(Method ctor, params object[] parameters)
        {
            // TODO

            this.instructions.Append(this.processor.Create(OpCodes.Throw));
            return this;
        }
    }
}