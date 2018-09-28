using Cauldron.Activator;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace Cauldron.XAML.Theme
{
    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Component(typeof(IFactoryExtension))]
    public class CauldronThemeInit : IFactoryExtension
    {
        /// <exclude/>
        public bool IsInitialized { get; private set; }

        /// <exclude/>
        public void Initialize(IEnumerable<IFactoryTypeInfo> factoryInfoTypes)
        {
            if (this.IsInitialized)
                return;

            this.IsInitialized = true;

            if (!ApplicationBase.Current.Resources.Contains(CauldronTheme.AccentColor))
                ApplicationBase.Current.Resources.Add(CauldronTheme.AccentColor, Colors.LightSteelBlue);
        }
    }
}