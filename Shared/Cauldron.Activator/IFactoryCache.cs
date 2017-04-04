using System.ComponentModel;

namespace Cauldron.Activator
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IFactoryCache
    {
        //object GetInstance(string contractName, object[] parameters);

        /// <exclude/>
        IFactoryTypeInfo[] GetComponents();
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}