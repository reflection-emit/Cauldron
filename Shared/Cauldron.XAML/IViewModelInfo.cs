using System;
using System.ComponentModel;

#if NETFX_CORE
using Windows.UI.Xaml.Data;
#else

using System.Windows.Data;

#endif

namespace Cauldron.XAML
{
    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IViewModelInfo
    {
        /// <exclude/>
        Type Type { get; }

        /// <exclude/>
        Type ViewType { get; }

        /// <exclude/>
        object CreateInstance();
    }
}