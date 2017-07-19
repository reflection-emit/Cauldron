using Cauldron.XAML;
using Cauldron.XAML.Theme;
using System;
using System.Threading.Tasks;
using System.Windows.Media;
using ThemeSample.ViewModels;

namespace ThemeSample
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

        protected override void OnResourceLoad()
        {
            CauldronTheme.SetAccentColor(Colors.SteelBlue);
        }

        protected override async Task OnStartup(LaunchActivatedEventArgs e)
        {
            await this.Navigator.NavigateAsync(typeof(MainViewModel));
        }
    }
}