using Cauldron.XAML;
using StandardApplication.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace StandardApplication
{
    public class App : ApplicationBase
    {
        public App()
        {
            this.IsSingleInstance = true;
            this.ShouldBringToFront = false;
        }

        [STAThread]
        public static void Main(string[] args)
        {
            var p = new App();
            p.Run();
        }

        protected override async Task OnStartup(LaunchActivatedEventArgs e)
        {
            await this.Navigator.NavigateAsync(typeof(MainViewModel));
        }
    }
}