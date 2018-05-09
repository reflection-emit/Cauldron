using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Activator
{
    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class FactoryCore
    {
        /// <exclude/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static IFactoryTypeInfo[] _components;

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetComponents(IFactoryTypeInfo[] assembly)
        {
            if (_components == null)
                _components = assembly;
            else
                _components = _components.Concat(assembly).Distinct(new FactoryTypeInfoComparer()).ToArray();
        }
    }
}