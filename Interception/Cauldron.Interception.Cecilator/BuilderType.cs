using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class BuilderType : BuilderBase
    {
        private TypeDefinition typeDefinition;
        private TypeReference typeReference;

        internal BuilderType(IWeaver weaver, TypeReference typeReference, TypeDefinition typeDefinition) : base(weaver)
        {
            this.typeDefinition = typeDefinition;
            this.typeReference = typeReference;
        }

        internal BuilderType(Builder builder, TypeDefinition typeDefinition) : base(builder)
        {
            this.typeDefinition = typeDefinition;
            this.typeReference = typeDefinition;
        }

        internal BuilderType(BuilderType builderType, TypeDefinition typeDefinition) : base(builderType)
        {
            this.typeDefinition = typeDefinition;
            this.typeReference = typeDefinition;
        }

        internal BuilderType(BuilderType builderType, TypeReference typeReference) : base(builderType)
        {
            this.typeReference = typeReference;
            this.typeDefinition = typeReference.Resolve();
        }

        public IEnumerable<BuilderType> Interfaces { get { return this.GetInterfaces(this.typeDefinition).Select(x => new BuilderType(this, x)); } }

        public void Remove() => this.moduleDefinition.Types.Remove(this.typeDefinition);

        public override string ToString() => this.typeReference.FullName;

        //public Method Create(string name, MethodAttributes attributes, TypeReference returnType)
        //{
        //}

        //public Method CreateStatic(string name, MethodAttributes attributes, TypeReference returnType)
        //{
        //}

        //public IEnumerable<Method> FindAll(string regexName)
        //{
        //}

        //public Method FindFirst(string name, params Type[] parameters)
        //{
        //}
    }
}