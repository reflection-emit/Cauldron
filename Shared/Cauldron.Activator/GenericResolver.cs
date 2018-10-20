using Cauldron.Reflection;
using System;
using System.Reflection;

namespace Cauldron.Activator
{
    /// <summary>
    /// Resolves generic contract types to its implementation.
    /// </summary>
    internal class GenericResolver : IFactoryTypeInfo
    {
        private readonly Type contractType;

        private ObjectActivator parameterlessActivator;

        public GenericResolver(Type contractType, IFactoryTypeInfo resolvesTo)
        {
            this.contractType = contractType;
            this.Type = resolvesTo.Type.MakeGenericType(contractType.GetGenericArguments());
            this.CreationPolicy = resolvesTo.CreationPolicy;
            this.Priority = resolvesTo.Priority;
        }

        public Type ChildType => null;

        public string ContractName => this.contractType.FullName;

        public Type ContractType => this.contractType;

        public FactoryCreationPolicy CreationPolicy { get; }

        public object Instance { get; set; }

        public bool IsEnumerable => false;

        public uint Priority { get; }

        public Type Type { get; }

        public virtual object CreateInstance()
        {
            if (this.parameterlessActivator == null)
            {
            }

            return this.parameterlessActivator();
        }

        public virtual object CreateInstance(params object[] arguments)
        {
            throw new NotImplementedException();
        }
    }

    internal class GenericResolverSingleton : GenericResolver
    {
        public GenericResolverSingleton(Type contractType, IFactoryTypeInfo resolvesTo) : base(contractType, resolvesTo)
        {
        }

        public override object CreateInstance()
        {
            if (this.Instance != null)
                return this.Instance;

            return base.CreateInstance();
        }

        public override object CreateInstance(params object[] arguments)
        {
            if (this.Instance != null)
                return this.Instance;

            return base.CreateInstance(arguments);
        }
    }
}