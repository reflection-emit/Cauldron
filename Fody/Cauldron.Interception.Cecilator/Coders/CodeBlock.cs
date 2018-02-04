using Mono.Cecil;
using System;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Coders
{
    public static class CodeBlocks
    {
        public static CodeBlock This => new ThisCodeBlock();

        public static CodeBlock CreateException(TypeReference typeReference, string name) => new ExceptionCodeBlock { name = name, typeReference = typeReference };

        public static CodeBlock CreateException(BuilderType builderType, string name) => new ExceptionCodeBlock { name = name, typeReference = builderType.typeReference };

        public static CodeBlock DefaultOfStruct(TypeReference typeReference) => new InitObjCodeBlock { typeReference = typeReference };

        public static CodeBlock DefaultOfTask(TypeReference typeReference) => new DefaultTaskCodeBlock { typeReference = typeReference };

        public static CodeBlock DefaultTaskOfT(TypeReference typeReference) => new DefaultTaskOfTCodeBlock { typeReference = typeReference };

        public static ParametersCodeBlock GetParameter(int index) => new ParametersCodeBlock { index = index };

        public static ParametersCodeBlock GetParameter(string name) => new ParametersCodeBlock { name = name };

        public static ParametersCodeBlock GetParameter() => new ParametersCodeBlock();
    }

    public class ArrayCodeBlock : CodeBlock
    {
        internal int index;

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

    public class ExceptionCodeBlock : CodeBlock
    {
        internal string name;

        internal TypeReference typeReference;

        internal ExceptionCodeBlock()
        {
        }
    }

    public class FieldAssignCoderCodeBlock : CodeBlock
    {
        internal readonly Coder coder;
        internal readonly FieldAssignCoder fieldAssignCoder;

        internal FieldAssignCoderCodeBlock(FieldAssignCoder fieldAssignCoder)
        {
            this.fieldAssignCoder = fieldAssignCoder;
            this.coder = fieldAssignCoder.coder;
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

    public class ParametersCodeBlock : CodeBlock
    {
        internal int? index;

        internal string name;

        internal ParametersCodeBlock()
        {
        }

        public bool IsAllParameters => !this.index.HasValue && string.IsNullOrEmpty(this.name);

        public Tuple<BuilderType, int> GetTargetType(Coder coder) => this.GetTargetType(coder.instructions.associatedMethod);

        public Tuple<BuilderType, int> GetTargetType(Method method)
        {
            if (this.IsAllParameters || method == null)
                return null;

            if (method.methodDefinition.Parameters.Count == 0)
                throw new ArgumentException($"The method {method.Name} does not have any parameters");

            if (this.index.HasValue)

                return new Tuple<BuilderType, int>(
                    method.methodDefinition.Parameters[this.index.Value].ParameterType.ToBuilderType().Import(),
                    method.IsStatic ? this.index.Value : this.index.Value + 1
                    );
            else
            {
                for (int i = 0; i < method.Parameters.Length; i++)
                    if (method.Parameters[i].Name == name)
                        return new Tuple<BuilderType, int>(
                            method.methodReference.Parameters[i].ParameterType.ToBuilderType().Import(),
                            method.IsStatic ? i : i + 1
                            );
            }

            return null;
        }

        public CodeBlock UnPacked(int arrayIndex = 0) => new ArrayCodeBlock { index = arrayIndex };
    }

    public class ThisCodeBlock : CodeBlock
    {
        internal ThisCodeBlock()
        {
        }
    }
}