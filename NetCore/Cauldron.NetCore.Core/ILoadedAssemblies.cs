using System.ComponentModel;
using System.Reflection;

namespace Cauldron.Core
{
    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface ILoadedAssemblies
    {
        /// <exclude/>
        Assembly[] ReferencedAssemblies();
    }
}