using Couldron;
using System.Windows;
using ViewModelCommunication.ViewModels;

namespace ViewModelCommunication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : CouldronApplication
    {
        protected override void OnConstruction()
        {
            AssemblyUtil.LoadAssembly("Couldron.Themes.VisualStudioLight.dll");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Navigator.Navigate<MainViewModel>();
        }
    }
}