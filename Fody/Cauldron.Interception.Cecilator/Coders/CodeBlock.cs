using Mono.Cecil;

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

        public CodeBlock UnPacked(int arrayIndex = 0) => new ArrayCodeBlock { index = arrayIndex };
    }

    public class ThisCodeBlock : CodeBlock
    {
        internal ThisCodeBlock()
        {
        }
    }
}