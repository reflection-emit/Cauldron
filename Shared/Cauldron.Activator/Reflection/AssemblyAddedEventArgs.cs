using System;
using System.Reflection;

namespace Cauldron.Reflection
{
    /// <summary>
    /// Contains data of the <see cref="Assemblies.LoadedAssemblyChanged"/> event.
    /// </summary>
    public sealed class AssemblyAddedEventArgs : EventArgs
    {
        internal AssemblyAddedEventArgs(Assembly assembly, MethodInfo cauldronGetComponentsMethod)
        {
            this.CauldronGetComponents = new MethodInfo[] { cauldronGetComponentsMethod };
            this.Assembly = new Assembly[] { assembly };
        }

        internal AssemblyAddedEventArgs(Assembly[] assembly, MethodInfo[] cauldronGetComponentsMethod)
        {
            this.CauldronGetComponents = cauldronGetComponentsMethod;
            this.Assembly = assembly;
        }

        /// <summary>
        /// Gets the assembly that has been added to the known assembly collection
        /// </summary>
        public Assembly[] Assembly { get; private set; }

        /// <summary>
        /// Gets the auto-generated cauldron object.
        /// </summary>
        public MethodInfo[] CauldronGetComponents { get; private set; }
    }
}