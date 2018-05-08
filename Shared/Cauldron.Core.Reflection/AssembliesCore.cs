using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace Cauldron.Core.Reflection
{
    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class AssembliesCore
    {
        /// <exclude/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static Assembly _entryAssembly;

        /// <exclude/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static Assembly[] _referencedAssemblies;

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetEntryAssembly(Assembly assembly)
        {
            if (_entryAssembly == null)
                _entryAssembly = assembly;
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetReferenceAssemblies(Assembly[] assemblies)
        {
            if (_referencedAssemblies == null)
                _referencedAssemblies = assemblies;
        }
    }
}