using System.ComponentModel;

namespace Cauldron.Interception
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IFactoryCache
    {
        object GetInstance(string contractName, object[] parameters);
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}