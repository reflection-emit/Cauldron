using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public sealed class Builder : BuilderBase
    {
        internal Builder(IWeaver weaver) : base(weaver)
        {
        }

        //public BuilderType Create(string name)
        //{
        //}

        //public BuilderType Create(string nameSpace, string name, TypeAttributes attributes)
        //{
        //}

        //public IEnumerable<BuilderType> FindAll(string regexName)
        //{
        //}

        //public BuilderType FindFirst(string name, params Type[] parameters)
        //{
        //}
        public override string ToString() => this.moduleDefinition.Assembly.FullName;

        public IEnumerable<BuilderType> TypesInNamespace(string namespaceName) =>
            this.moduleDefinition.Types.Where(x => x.Namespace == namespaceName).Select(x => new BuilderType(this, x)).ToArray();

        public IEnumerable<BuilderType> TypesWithInterface(Type interfaceType)
        {
            if (!interfaceType.IsInterface)
                throw new ArgumentException("Argument 'interfaceType' is not an interface");

            return this.TypesWithInterface(interfaceType.FullName);
        }

        public IEnumerable<BuilderType> TypesWithInterface(string interfaceName) =>
            this.moduleDefinition.Types.Where(x => this.ImplementsInterface(x, interfaceName)).Select(x => new BuilderType(this, x)).ToArray();
    }
}