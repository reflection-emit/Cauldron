using System;
using System.Reflection;

namespace Cauldron.Core.Reflection
{
    /// <summary>
    /// Contains data of the <see cref="Assemblies.LoadedAssemblyChanged"/> event.
    /// </summary>
    public sealed class AssemblyAddedEventArgs : EventArgs
    {
        internal AssemblyAddedEventArgs(Assembly assembly, object cauldron)
        {
            this.Cauldron = cauldron;
            this.Assembly = assembly;
        }

        /// <summary>
        /// Gets the assembly that has been added to the known assembly collection
        /// </summary>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// Gets the auto-generated cauldron object.
        /// </summary>
        public object Cauldron { get; private set; }
    }
}