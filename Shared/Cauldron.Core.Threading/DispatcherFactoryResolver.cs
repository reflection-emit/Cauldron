using Cauldron.Activator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Core.Threading
{
    /// <summary>
    /// Automatically selects the proper implementation of <see cref="IDispatcher"/>
    /// </summary>
    public sealed class DispatcherFactoryResolver : UnitTestAwareFactoryResolverBase
    {
        private const string ContractName = "Cauldron.Core.Threading.IDispatcher";

        private static IFactoryTypeInfo dispatcher;

        /// <summary>
        /// Called during the initialization of the Factory.
        /// </summary>
        /// <param name="factoryInfoTypes">A collection of known factory types.</param>
        protected override void OnInitialize(IEnumerable<IFactoryTypeInfo> factoryInfoTypes)
        {
            Factory.Resolvers.Add(ContractName, new Func<IFactoryTypeInfo>(() =>
            {
                if (dispatcher != null)
                    return dispatcher;

                if (this.IsUnitTest)
                    dispatcher = factoryInfoTypes.FirstOrDefault(x => x.Type == typeof(DispatcherDummy));
                else
                    dispatcher = factoryInfoTypes.Where(x => x.ContractName == ContractName).MaxBy(x => x.Priority);

                return dispatcher;
            }));
        }
    }
}