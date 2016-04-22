using Cauldron;
using System;
using System.Windows;
using System.Windows.Media;
using ViewModelCommunication.ViewModels;

namespace ViewModelCommunication
{
    public class App : CauldronApplication
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new App();
            app.Run();
        }

        protected override void OnConstruction()
        {
            AssemblyUtil.LoadAssembly("Cauldron.Themes.VisualStudioLight.dll");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Navigator.Navigate<MainViewModel>();
        }
    }
}