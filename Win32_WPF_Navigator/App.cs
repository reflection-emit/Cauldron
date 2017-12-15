using Cauldron.Activator;
using Cauldron.XAML;
using System;
using System.Threading.Tasks;

namespace Win32_WPF_Navigator
{
    public class App : ApplicationBase
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                new App().Run();
            }
            catch (Exception e)
            {
                Factory.Create<IMessageDialog>().ShowExceptionAsync(e);
            }
        }

        protected override Task OnStartup(LaunchActivatedEventArgs e)
        {
            return this.Navigator.NavigateAsync<MainViewModel>("A parameter", 42);
        }
    }
}