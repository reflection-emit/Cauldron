using System.ComponentModel;
using System.Reflection;
using Windows.UI.Xaml;

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