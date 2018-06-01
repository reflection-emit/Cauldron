using System;
using System.ComponentModel;

namespace Cauldron.Activator
{
    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class FactoryTypeInfoInternal : IFactoryTypeInfo
    {
        private Func<object[], object> createInstance;
        private object instanceObject = new object();

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
        public Type ContractType { get; }

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
        public object CreateInstance(params object[] arguments)
        {
            if (this.CreationPolicy == FactoryCreationPolicy.Instanced)
            {
                if (arguments == null || arguments.Length == 0)
                    return this.createInstance(null);

                return this.createInstance(arguments);
            }

            if (this.Instance == null)
            {
                lock (this.instanceObject)
                {
                    if (arguments == null || arguments.Length == 0)
                        this.Instance = this.createInstance(null);
                    else
                        this.Instance = this.createInstance(arguments);
                }
            }

            return this.Instance;
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object CreateInstance()
        {
            if (this.CreationPolicy == FactoryCreationPolicy.Instanced)
                return this.createInstance(null);

            if (this.Instance == null)
                lock (this.instanceObject)
                {
                    this.Instance = this.createInstance(null);
                }

            return this.Instance;
        }
    }
}