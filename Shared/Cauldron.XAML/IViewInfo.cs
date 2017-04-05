using System;
using System.ComponentModel;
using System.Windows;

#if NETFX_CORE
using Windows.UI.Xaml.Data;
#else

using System.Windows.Data;

#endif

namespace Cauldron.XAML
{
    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IViewInfo
    {
        /// <exclude/>
        Type Type { get; }

        /// <exclude/>
        FrameworkElement CreateInstance();
    }
}