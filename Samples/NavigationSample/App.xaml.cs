using Cauldron;
using NavigationSample.ViewModels;
using System.Windows;
using System.Windows.Media;

namespace NavigationSample
{
    public partial class App : CauldronApplication
    {
        protected override void OnConstruction()
        {
            AssemblyUtil.LoadAssembly("Cauldron.Themes.VisualStudioDark.dll");

            this.ThemeAccentColor = Colors.Tomato;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Navigator.Navigate<MainViewModel>();
        }
    }
}