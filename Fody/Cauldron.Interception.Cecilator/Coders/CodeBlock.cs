using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.IO;

namespace Cauldron.Interception.Cecilator.Coders
{
    public static class CodeBlocks
    {
        public const string ReturnVariableName = "<>returnValue";
        public const string VariablePrefix = "<>var_";
        public static CodeBlock This => new ThisCodeBlock();

        public static CodeBlock CreateException(TypeReference typeReference, string name) => new ExceptionCodeBlock { name = name, typeReference = typeReference };

        public static CodeBlock CreateException(BuilderType builderType, string name) => new ExceptionCodeBlock { name = name, typeReference = builderType.typeReference };

        public static CodeBlock DefaultOfStruct(TypeReference typeReference) => new InitObjCodeBlock { typeReference = typeReference };

        public static CodeBlock DefaultOfTask(TypeReference typeReference) => new DefaultTaskCodeBlock { typeReference = typeReference };

        public static CodeBlock DefaultTaskOfT(TypeReference typeReference) => new DefaultTaskOfTCodeBlock { typeReference = typeReference };

        public static CodeBlock DefaultValueOf(BuilderType builderType) => new DefaultValueCodeBlock { builderType = builderType };

        /// <summary>
        /// Generates a random name that can be used to name variables and methods.
        /// </summary>
        /// <returns></returns>
        public static string GenerateName() => Path.GetRandomFileName().Replace(".", DateTime.Now.Second.ToString());

        public static ParametersCodeBlock GetParameter(int index) => new ParametersCodeBlock { index = index };

        public static ParametersCodeBlock GetParameter(string name) => new ParametersCodeBlock { name = name };

        public static ParametersCodeBlock GetParameters() => new ParametersCodeBlock();
    }

    public class ArrayCodeBlock : CodeBlock
    {
        internal ArrayCodeBlock()
        {
        }
    }

    public class CodeBlock
    {
    }

    public class DefaultTaskCodeBlock : CodeBlock
    {
        internal TypeReference typeReference;

        internal DefaultTaskCodeBlock()
        {
        }
    }

    public class DefaultTaskOfTCodeBlock : CodeBlock
    {
        internal TypeReference typeReference;

        internal DefaultTaskOfTCodeBlock()
        {
        }
    }

    public class DefaultValueCodeBlock : CodeBlock
    {
        internal BuilderType builderType;

        internal DefaultValueCodeBlock()
        {
        }
    }

    public class ExceptionCodeBlock : CodeBlock
    {
        internal string name;

        internal TypeReference typeReference;

        internal ExceptionCodeBlock()
        {
        }
    }

    public class InitObjCodeBlock : CodeBlock
    {
        internal TypeReference typeReference;

        internal InitObjCodeBlock()
        {
        }
    }

    public class InstructionsCodeBlock : CodeBlock
    {
        internal readonly InstructionBlock instructions;

        internal InstructionsCodeBlock(Coder coder) => this.instructions = coder.instructions;
    }

    public class NullableCodeBlock : CodeBlock
    {
        internal readonly BuilderType builderType;
        internal readonly object value;
        internal readonly VariableDefinition variable;

        internal NullableCodeBlock(VariableDefinition variable, BuilderType builderType) : this(null, builderType, variable)
        {
        }

        internal NullableCodeBlock(object value, BuilderType builderType) : this(value, builderType, null)
        {
        }

        private NullableCodeBlock(object value, BuilderType builderType, VariableDefinition variable)
        {
            this.variable = variable;
            this.value = value;
            this.builderType = builderType;
        }
    }

    public class ParameterArrayCodeBlock : ArrayCodeBlock
    {
        internal int index;

        internal ParameterArrayCodeBlock()
        {
        }
    }

    public class ParametersCodeBlock : CodeBlock
    {
        internal int? index;

        internal string name;

        internal ParametersCodeBlock()
        {
        }

        public bool IsAllParameters => !this.index.HasValue && string.IsNullOrEmpty(this.name);

        public static ParameterReference GetParameter(Method method, int parameterIndex) => GetParameterReference(method, parameterIndex);

        public static ParameterReference GetParameter(Method method, Instruction instruction)
        {
            var argOffset = method.IsStatic ? 0 : 1;

            if (instruction.OpCode == OpCodes.Ldarg || instruction.OpCode == OpCodes.Ldarga || instruction.OpCode == OpCodes.Ldarga_S)
                switch (instruction.Operand)
                {
                    case ParameterDefinition value: return value;
                    case ParameterReference value: return value;
                    case int value: return GetParameterReference(method, value - argOffset);
                }

            if (instruction.OpCode == OpCodes.Ldarg_0) return GetParameterReference(method, 0 - argOffset);
            if (instruction.OpCode == OpCodes.Ldarg_1) return GetParameterReference(method, 1 - argOffset);
            if (instruction.OpCode == OpCodes.Ldarg_2) return GetParameterReference(method, 2 - argOffset);
            if (instruction.OpCode == OpCodes.Ldarg_3) return GetParameterReference(method, 3 - argOffset);

            return null;
        }

        /// <summary>
        /// Loads all elements of an array one by one to the stack.
        /// </summary>
        /// <returns></returns>
        public ArrayCodeBlock ArrayElements() => new ParameterArrayCodeBlock
        {
            index = this.index.Value
        };

        public Tuple<BuilderType, int, ParameterDefinition> GetTargetType(Coder coder) => this.GetTargetType(coder.instructions.associatedMethod);

        public Tuple<BuilderType, int, ParameterDefinition> GetTargetType(Method method)
        {
            if (this.IsAllParameters || method == null)
                return null;

            if (method.methodDefinition.Parameters.Count == 0)
                throw new ArgumentException($"The method {method.Name} does not have any parameters");

            if (this.index.HasValue)
            {
                if (!method.IsStatic && this.index.Value < 0)
                    return new Tuple<BuilderType, int, ParameterDefinition>(method.OriginType, -1, null);

                return new Tuple<BuilderType, int, ParameterDefinition>(
                    method.methodDefinition.Parameters[this.index.Value].ParameterType.ToBuilderType().Import(),
                    method.IsStatic ? this.index.Value : this.index.Value + 1,
                    method.methodDefinition.Parameters[this.index.Value]);
            }
            else
            {
                for (int i = 0; i < method.Parameters.Length; i++)
                    if (method.Parameters[i].Name == name)
                        return new Tuple<BuilderType, int, ParameterDefinition>(
                            method.methodReference.Parameters[i].ParameterType.ToBuilderType().Import(),
                            method.IsStatic ? i : i + 1,
                            method.methodReference.Parameters[i]);
            }

            return null;
        }

        internal static ParameterReference GetParameterReference(Method method, int parameterIndex)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            if (parameterIndex < 0)
                return null;

            return method.methodDefinition.Parameters[parameterIndex];
        }

        internal static BuilderType GetTargetType(Method method, int parameterIndex)
        {
            var result = GetParameterReference(method, parameterIndex);

            if (result == null)
                return method.OriginType;

            return result.ParameterType.ToBuilderType().Import();
        }

        internal static BuilderType GetTargetTypeFromOpCode(Method method, Instruction instruction)
        {
            var argOffset = method.IsStatic ? 0 : 1;

            if (instruction.OpCode == OpCodes.Ldarg || instruction.OpCode == OpCodes.Ldarga || instruction.OpCode == OpCodes.Ldarga_S)
                switch (instruction.Operand)
                {
                    case ParameterDefinition value: return GetTargetType(method, value.Index);
                    case ParameterReference value: return GetTargetType(method, value.Index);
                    case int value: return GetTargetType(method, value - argOffset);
                }

            if (instruction.OpCode == OpCodes.Ldarg_0) return GetTargetType(method, 0 - argOffset);
            if (instruction.OpCode == OpCodes.Ldarg_1) return GetTargetType(method, 1 - argOffset);
            if (instruction.OpCode == OpCodes.Ldarg_2) return GetTargetType(method, 2 - argOffset);
            if (instruction.OpCode == OpCodes.Ldarg_3) return GetTargetType(method, 3 - argOffset);

            return null;
        }
    }

    public class ParametersVariableCodeBlock : CodeBlock
    {
        internal readonly VariableDefinition variable;

        internal ParametersVariableCodeBlock(VariableDefinition variable) => this.variable = variable;
    }

    public class ThisCodeBlock : CodeBlock
    {
        internal ThisCodeBlock()
        {
        }
    }
}