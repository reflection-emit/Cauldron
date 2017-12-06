using System.ComponentModel;

namespace Cauldron.Activator
{
    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IFactoryCache
    {
        /// <exclude/>
        IFactoryTypeInfo[] GetComponents();
    }
}