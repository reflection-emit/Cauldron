using Cauldron;
using System;
using System.Windows;

namespace ControlsPlayground
{
    public class App : CauldronApplication
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                var app = new App();
                app.Run();
            }
            catch (Exception e)
            {
                // TODO - Logging
            }
        }

        protected override void OnConstruction()
        {
            AssemblyUtil.LoadAssembly("Cauldron.Themes.VisualStudioLight.dll");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // TODO
            //  Navigator.Navigate<MainViewModel>();
        }
    }
}