using System.Diagnostics;
using System.Reflection;

namespace Cauldron.Reflection
{
    /// <summary>
    /// Represents a resource info of an embedded resource with its corresponding <see cref="Assembly"/>
    /// </summary>
    [DebuggerDisplay("Filename = {Filename}")]
    public sealed class AssemblyResource
    {
        internal AssemblyResource(Assembly assembly, string filename)
        {
            this.Assembly = assembly;
            this.Filename = filename;
        }

        /// <summary>
        /// Gets the Assembly that contains the embedded resource
        /// </summary>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// Gets the fully qualified name of the embedded resource
        /// </summary>
        public string Filename { get; private set; }
    }
}