using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

#if NETFX_CORE
using Windows.UI.Xaml.Data;
#else

using System.Windows.Data;

#endif

namespace Cauldron.XAML
{
    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IXAMLCache
    {
        /// <exclude/>
        ResourceDictionary[] GetResourceDictionaries();

        /// <exclude/>
        IValueConverter[] GetValueConverters();

        /// <exclude/>
        IViewModelInfo[] GetViewModels();

        /// <exclude/>
        IViewInfo[] GetViews();
    }
}