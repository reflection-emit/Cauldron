using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace Cauldron.Core.Reflection
{
    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class AssembliesCore
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static Assembly _entryAssembly;

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetEntryAssembly(Assembly assembly)
        {
            if (_entryAssembly == null)
                _entryAssembly = assembly;
        }
    }
}