using Cauldron.Core.Reflection;
using System.Collections.Generic;

namespace Cauldron.Activator
{
    /// <summary>
    /// Represents an interface for the <see cref="Factory"/> extension
    /// </summary>
    public interface IFactoryExtension
    {
        /// <summary>
        /// Called after Factory initialization. It is also called if <see cref="Assemblies.LoadedAssemblyChanged"/> has been executed.
        /// This will be only called one time per extension only.
        /// </summary>
        /// <param name="factoryInfoTypes">A collection of known factory types.</param>
        void Initialize(IEnumerable<IFactoryTypeInfo> factoryInfoTypes);
    }
}