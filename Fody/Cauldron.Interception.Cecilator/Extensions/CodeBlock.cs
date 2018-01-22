using Mono.Cecil;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public class ArrayCodeSet : CodeBlock
    {
        internal int index;

        internal ArrayCodeSet()
        {
        }
    }

    public class CodeBlock
    {
        public static CodeBlock This => new ThisCodeSet();

        public static CodeBlock CreateException(TypeReference typeReference, string name) => new ExceptionCodeSet { name = name, typeReference = typeReference };

        public static CodeBlock CreateException(BuilderType builderType, string name) => new ExceptionCodeSet { name = name, typeReference = builderType.typeReference };

        public static CodeBlock DefaultOfStruct(TypeReference typeReference) => new InitObjCodeSet { typeReference = typeReference };

        public static CodeBlock DefaultOfTask(TypeReference typeReference) => new DefaultTaskCodeSet { typeReference = typeReference };

        public static CodeBlock DefaultTaskOfT(TypeReference typeReference) => new DefaultTaskOfTCodeSet { typeReference = typeReference };

        public static ParametersCodeSet GetParameter(int index) => new ParametersCodeSet { index = index };

        public static ParametersCodeSet GetParameter(string name) => new ParametersCodeSet { name = name };

        public static ParametersCodeSet GetParameter() => new ParametersCodeSet();
    }

    public class DefaultTaskCodeSet : CodeBlock
    {
        internal TypeReference typeReference;

        internal DefaultTaskCodeSet()
        {
        }
    }

    public class DefaultTaskOfTCodeSet : CodeBlock
    {
        internal TypeReference typeReference;

        internal DefaultTaskOfTCodeSet()
        {
        }
    }

    public class ExceptionCodeSet : CodeBlock
    {
        internal string name;

        internal TypeReference typeReference;

        internal ExceptionCodeSet()
        {
        }
    }

    public class FieldAssignCoderCodeSet : CodeBlock
    {
        internal readonly Coder coder;
        internal readonly FieldAssignCoder fieldAssignCoder;

        internal FieldAssignCoderCodeSet(FieldAssignCoder fieldAssignCoder)
        {
            this.fieldAssignCoder = fieldAssignCoder;
            this.coder = fieldAssignCoder.coder;
        }
    }

    public class InitObjCodeSet : CodeBlock
    {
        internal TypeReference typeReference;

        internal InitObjCodeSet()
        {
        }
    }

    public class InstructionsCodeSet : CodeBlock
    {
        internal readonly InstructionContainer instructions;

        internal InstructionsCodeSet(Coder coder)
        {
            this.instructions = coder.instructions;
        }
    }

    public class ParametersCodeSet : CodeBlock
    {
        internal int? index;

        internal string name;

        internal ParametersCodeSet()
        {
        }

        public bool IsAllParameters => !this.index.HasValue && string.IsNullOrEmpty(this.name);

        public CodeBlock UnPacked(int arrayIndex = 0) => new ArrayCodeSet { index = arrayIndex };
    }

    public class ThisCodeSet : CodeBlock
    {
        internal ThisCodeSet()
        {
        }
    }
}