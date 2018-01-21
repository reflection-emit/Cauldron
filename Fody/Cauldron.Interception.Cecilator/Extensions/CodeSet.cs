using Mono.Cecil;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public class ArrayCodeSet : CodeSet
    {
        internal int index;

        internal ArrayCodeSet()
        {
        }
    }

    public class CoderCodeSet : CodeSet
    {
        internal readonly Coder coder;

        internal CoderCodeSet(Coder coder)
        {
            this.coder = coder;
        }
    }

    public class CodeSet
    {
        public static CodeSet This => new ThisCodeSet();

        public static CodeSet CreateException(TypeReference typeReference, string name) => new ExceptionCodeSet { name = name, typeReference = typeReference };

        public static CodeSet CreateException(BuilderType builderType, string name) => new ExceptionCodeSet { name = name, typeReference = builderType.typeReference };

        public static CodeSet DefaultOfStruct(TypeReference typeReference) => new InitObjCodeSet { typeReference = typeReference };

        public static CodeSet DefaultOfTask(TypeReference typeReference) => new DefaultTaskCodeSet { typeReference = typeReference };

        public static CodeSet DefaultTaskOfT(TypeReference typeReference) => new DefaultTaskOfTCodeSet { typeReference = typeReference };

        public static ParametersCodeSet GetParameter(int index) => new ParametersCodeSet { index = index };

        public static ParametersCodeSet GetParameter(string name) => new ParametersCodeSet { name = name };

        public static ParametersCodeSet GetParameter() => new ParametersCodeSet();
    }

    public class DefaultTaskCodeSet : CodeSet
    {
        internal TypeReference typeReference;

        internal DefaultTaskCodeSet()
        {
        }
    }

    public class DefaultTaskOfTCodeSet : CodeSet
    {
        internal TypeReference typeReference;

        internal DefaultTaskOfTCodeSet()
        {
        }
    }

    public class ExceptionCodeSet : CodeSet
    {
        internal string name;

        internal TypeReference typeReference;

        internal ExceptionCodeSet()
        {
        }
    }

    public class FieldAssignCoderCodeSet : CodeSet
    {
        internal readonly Coder coder;
        internal readonly FieldAssignCoder fieldAssignCoder;

        internal FieldAssignCoderCodeSet(FieldAssignCoder fieldAssignCoder)
        {
            this.fieldAssignCoder = fieldAssignCoder;
            this.coder = fieldAssignCoder.coder;
        }
    }

    public class InitObjCodeSet : CodeSet
    {
        internal TypeReference typeReference;

        internal InitObjCodeSet()
        {
        }
    }

    public class ParametersCodeSet : CodeSet
    {
        internal int? index;

        internal string name;

        internal ParametersCodeSet()
        {
        }

        public bool IsAllParameters => !this.index.HasValue && string.IsNullOrEmpty(this.name);

        public CodeSet UnPacked(int arrayIndex = 0) => new ArrayCodeSet { index = arrayIndex };
    }

    public class ThisCodeSet : CodeSet
    {
        internal ThisCodeSet()
        {
        }
    }
}