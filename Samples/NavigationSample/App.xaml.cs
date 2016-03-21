using Couldron;
using NavigationSample.ViewModels;
using System.Windows;

namespace NavigationSample
{
    public partial class App : CouldronApplication
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Navigator.Navigate<MainViewModel>();
        }
    }
}