using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public static class MethodBodyCoderExtensions
    {
        public static LocalVariable CreateVariable(this Coder coder, string name, Method method)
        {
            if (method.IsCtor)
                return coder.CreateVariable(name, method.OriginType);

            return coder.CreateVariable(name, method.ReturnType);
        }

        public static LocalVariable CreateVariable(this Coder coder, string name, BuilderType type)
        {
            var existingVariable = coder.method.GetLocalVariable(name);

            if (existingVariable != null)
                return new LocalVariable(coder.method.type, existingVariable, name);

            var newVariable = new VariableDefinition(Builder.Current.Import(type.typeReference));
            coder.method.AddLocalVariable(name, newVariable);

            return new LocalVariable(coder.method.type, newVariable, name);
        }

        public static LocalVariable CreateVariable(this Coder coder, Type type)
        {
            var newVariable = new VariableDefinition(Builder.Current.Import(type.GetTypeDefinition()));
            var name = Coder.VariablePrefix + Coder.GenerateName();

            coder.method.AddLocalVariable(name, newVariable);
            return new LocalVariable(coder.method.type, newVariable, name);
        }

        public static LocalVariable CreateVariable(this Coder coder, Method method)
        {
            if (method.IsCtor)
                return coder.CreateVariable(method.OriginType);

            return coder.CreateVariable(method.ReturnType);
        }

        public static LocalVariable CreateVariable(this Coder coder, TypeReference type)
        {
            var newVariable = new VariableDefinition(Builder.Current.Import(type));
            var name = Coder.VariablePrefix + Coder.GenerateName();

            coder.method.AddLocalVariable(name, newVariable);
            return new LocalVariable(coder.method.type, newVariable, name);
        }

        public static LocalVariable CreateVariable(this Coder coder, BuilderType type)
        {
            var newVariable = new VariableDefinition(Builder.Current.Import(type.typeReference));
            var name = Coder.VariablePrefix + Coder.GenerateName();

            coder.method.AddLocalVariable(name, newVariable);
            return new LocalVariable(coder.method.type, newVariable, name);
        }

        /// <summary>
        /// Gets or creates a return variable.
        /// This will try to detect the existing return variable and create a new return variable if not found.
        /// </summary>
        /// <param name="coder">The coder to use.</param>
        /// <returns>A return variable.</returns>
        public static VariableDefinition GetOrCreateReturnVariable(this Coder coder)
        {
            var variable = coder.method.GetLocalVariable(Coder.ReturnVariableName);

            if (variable != null)
                return variable;

            if (coder.method.methodDefinition.Body.Instructions.Count > 1)
            {
                var lastOpCode = coder.method.methodDefinition.Body.Instructions.Last().Previous;

                if (lastOpCode.IsLoadLocal())
                {
                    if (lastOpCode.Operand is int index && coder.method.methodDefinition.Body.Variables.Count > index)
                        variable = coder.method.methodDefinition.Body.Variables[index];

                    if (variable == null && lastOpCode.Operand is VariableDefinition variableReference)
                        variable = variableReference;

                    if (variable == null)
                        if (lastOpCode.OpCode == OpCodes.Ldloc_0) variable = coder.method.methodDefinition.Body.Variables[0];
                        else if (lastOpCode.OpCode == OpCodes.Ldloc_1) variable = coder.method.methodDefinition.Body.Variables[1];
                        else if (lastOpCode.OpCode == OpCodes.Ldloc_2) variable = coder.method.methodDefinition.Body.Variables[2];
                        else if (lastOpCode.OpCode == OpCodes.Ldloc_3) variable = coder.method.methodDefinition.Body.Variables[3];

                    if (variable != null)
                    {
                        coder.method.AddLocalVariable(Coder.ReturnVariableName, variable);
                        return variable;
                    }
                }
            }

            return coder.method.AddLocalVariable(Coder.ReturnVariableName, new VariableDefinition(coder.method.ReturnType.typeReference));
        }

        public static bool HasReturnVariable(this Coder coder) => coder.method.GetLocalVariable(Coder.ReturnVariableName) != null;

        public static Coder ThrowNew(this Coder coder, Type exception)
        {
            coder.instructions.Append(coder.processor.Create(OpCodes.Newobj, Builder.Current.Import(Builder.Current.Import(exception).GetMethodReference(".ctor", 0))));
            coder.instructions.Append(coder.processor.Create(OpCodes.Throw));
            return coder;
        }

        public static Coder ThrowNew(this Coder coder, Type exception, string message)
        {
            coder.instructions.Append(coder.processor.Create(OpCodes.Ldstr, message));
            coder.instructions.Append(coder.processor.Create(OpCodes.Newobj, Builder.Current.Import(Builder.Current.Import(exception).GetMethodReference(".ctor", new Type[] { typeof(string) }))));
            coder.instructions.Append(coder.processor.Create(OpCodes.Throw));
            return coder;
        }
    }
}