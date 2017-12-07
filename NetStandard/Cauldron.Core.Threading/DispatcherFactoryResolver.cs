using Cauldron.Activator;
using System.Linq;

namespace Cauldron.Core.Threading
{
    /// <summary>
    /// Automatically selects the proper implementation of <see cref="IDispatcher"/>
    /// </summary>
    public sealed class DispatcherFactoryResolver : UnitTestAwareFactoryResolverBase
    {
        private const string ContractName = "Cauldron.Core.Threading.IDispatcher";

        /// <exclude />
        protected override bool IsApplicable(string contractName)
        {
            if (ContractName.GetHashCode() == contractName.GetHashCode() && ContractName == contractName)
                return true;

            return false;
        }

        /// <exclude />
        protected override IFactoryTypeInfo OnSelectAmbiguousMatch(IFactoryTypeInfo[] ambiguousTypes, string contractName)
        {
            if (this.IsUnitTest)
                return ambiguousTypes.FirstOrDefault(x => x.Type == typeof(DispatcherDummy));

            return ambiguousTypes.MaxBy(x => x.Priority);
        }
    }
}