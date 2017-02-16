using Mono.Cecil;

namespace Cauldron.Interception.Cecilator
{
    public class Method : BuilderBase
    {
        private MethodDefinition methodDefinition;
        private MethodReference methodReference;
        private BuilderType builderType;

        internal Method(IWeaver weaver, BuilderType builderType, MethodReference methodReference, MethodDefinition methodDefinition) : base(weaver)
        {
            this.builderType = builderType;
            this.methodDefinition = methodDefinition;
            this.methodReference = methodReference;
        }

        public Method Clear(MethodClearOptions options)
        {
            if (options.HasFlag(MethodClearOptions.Body))
                this.methodDefinition.Body.Instructions.Clear();

            if (options.HasFlag(MethodClearOptions.LocalVariables))
                this.methodDefinition.Body.Variables.Clear();

            return this;
        }

        public Method Clear() => this.Clear(MethodClearOptions.Body);
    }
}