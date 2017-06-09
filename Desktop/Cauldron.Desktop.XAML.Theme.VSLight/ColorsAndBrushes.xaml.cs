using Cauldron.Activator;
using System.Windows;
using System.Windows.Media;

namespace Cauldron.XAML.Theme.VSLight
{
    [Component(typeof(ResourceDictionary), uint.MaxValue)]
    public partial class ColorsAndBrushes
    {
        public ColorsAndBrushes()
        {
            if (!ApplicationBase.Current.Resources.Contains(CauldronTheme.AccentColor))
                ApplicationBase.Current.Resources.Add(CauldronTheme.AccentColor, Colors.LightSteelBlue);

            InitializeComponent();
        }
    }
}