using System;
using System.ComponentModel;

namespace Cauldron.Activator
{
    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class FactoryTypeInfoInternal : IFactoryTypeInfo
    {
        private Func<object[], object> createInstance;

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal FactoryTypeInfoInternal(string contractName, FactoryCreationPolicy creationPolicy, Type type, Func<object[], object> createInstance)
        {
            this.ContractName = contractName;
            this.CreationPolicy = creationPolicy;
            this.Type = type;
            this.createInstance = createInstance;
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ContractName { get; private set; }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public FactoryCreationPolicy CreationPolicy { get; private set; }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Instance { get; set; }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public uint Priority { get; private set; } = 0;

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Type Type { get; private set; }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object CreateInstance(params object[] arguments) => this.createInstance(arguments);
    }
}