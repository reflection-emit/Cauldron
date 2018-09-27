using Cauldron.Activator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.XAML.Threading
{
    /// <summary>
    /// Automatically selects the proper implementation of <see cref="IDispatcher"/>
    /// </summary>
    public sealed class DispatcherFactoryResolver : UnitTestAwareFactoryResolverBase
    {
        private static readonly string ContractName ;

        private static IFactoryTypeInfo dispatcher;

        static DispatcherFactoryResolver() => ContractName = typeof(IDispatcher).FullName;

        /// <exclude />
        [ComponentConstructor]
        internal DispatcherFactoryResolver()
        {
        }

        /// <summary>
        /// Called during the initialization of the Factory.
        /// </summary>
        /// <param name="factoryInfoTypes">A collection of known factory types.</param>
        protected override void OnInitialize(IEnumerable<IFactoryTypeInfo> factoryInfoTypes)
        {
            Factory.Resolvers.Add(ContractName, new Func<Type, IFactoryTypeInfo[], IFactoryTypeInfo>((callingType, ambigiousTypes) =>
             {
                 if (dispatcher != null)
                     return dispatcher;

                 if (this.IsUnitTest)
                     dispatcher = ambigiousTypes.FirstOrDefault(x => x.Type == typeof(DispatcherDummy));
                 else
                     dispatcher = ambigiousTypes.Where(x => x.ContractName == ContractName).MaxBy(x => x.Priority);

                 return dispatcher;
             }));
        }
    }
}