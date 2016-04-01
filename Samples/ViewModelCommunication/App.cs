using Couldron;
using System;
using System.Windows;
using System.Windows.Media;
using ViewModelCommunication.ViewModels;

namespace ViewModelCommunication
{
    public class App : CouldronApplication
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new App();
            app.Run();
        }

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